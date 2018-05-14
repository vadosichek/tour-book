from kivy.clock import Clock
from kivy.uix.floatlayout import FloatLayout
from kivy.uix.gridlayout import GridLayout
from kivy.properties import StringProperty
from kivy.properties import NumericProperty
from kivy.core.window import Window
from kivy.uix.scrollview import ScrollView
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.button import Button
from server import server
from server import USER_ID

from threading import Thread

from kivy.uix.screenmanager import ScreenManager, Screen
screenManager = ScreenManager()

class ScreenController():
    opened_post = None
    opened_profile = None
    screens = []

    def save_last(self, name):
        self.screens.append(name)
        print('saved', name)

    def open_post(self, post):
        self.opened_post.p_load(post)
        screenManager.current = 'OpenedPost'
        self.save_last('OpenedPost')

    def open_user(self, user):
        self.opened_profile.load(user)
        screenManager.current = 'Profile'
        self.save_last('Profile')

    def open_search(self):
        screenManager.current = 'Search'
        self.save_last('Search')

    def go_back(self):
        self.screens.pop(-1)
        screenManager.current = self.screens[-1] 

screenController = ScreenController()

class OpenedPost(Screen):
    base_layout = None
    loaded_comments = None

    def download_post(self, post):
        downloaded_post = Post()
        downloaded_post.load(post)
        return downloaded_post

    def p_load(self, post):
        self.base_layout.clear_widgets()
        self.base_layout.add_widget(GoBack())
        loading = Thread(target=self.load, args=(post,))
        loading.start()

    def load(self, post):
        self.base_layout.clear_widgets()
        self.base_layout.add_widget(GoBack())
        if type(post) is int:
            self.base_layout.add_widget(self.download_post(post))
            self.base_layout.add_widget(self.generate_comment_editor(post))
            comments = self.generate_comments(post)
        else:
            self.base_layout.add_widget(self.generate_post(post))
            self.base_layout.add_widget(self.generate_comment_editor(post.post))
            comments = self.generate_comments(post.post)

        for comment in comments:
            loaded_comment = Comment()
            loaded_comment.load(comment['user_name'], comment['text'])
            self.base_layout.add_widget(loaded_comment)
        
    def generate_post(self, post):
        opened_post = Post()
        opened_post.copy(post)
        return opened_post

    def generate_comment_editor(self, post):
        return CommentEditor().layout(post)

    def generate_comments(self, post):
        return server.get_comments(post)

    def __init__(self, **kwargs):
        super(OpenedPost, self).__init__(**kwargs)
        self.base_layout = GridLayout(cols=1, spacing=0, size_hint_y=None)
        self.base_layout.bind(minimum_height=self.base_layout.setter('height'))
        
        mainWidget = ScrollView(size_hint=(
            1, None), size=(Window.width, Window.height))
        mainWidget.add_widget(self.base_layout)
        self.add_widget(mainWidget)


class Feed(Screen):
    loaded_posts = []
    posts_layout = None
    posts_scroll = None

    def get_feed(self):
        return server.get_feed(USER_ID)

    def generate_posts(self, feed):
        self.loaded_posts = []
        for post_id in feed:
            post = Post()
            self.loaded_posts.append([post_id,post])
            self.posts_layout.add_widget(post)

    def load_posts(self):
        for post in self.loaded_posts:
            post[1].load(post[0])

    def generate_root(self, layout):
        root = ScrollView(size_hint=(1, None), size=(
            Window.width, Window.height))
        root.add_widget(layout)
        return root

    def generate_floating_button(self):
        actionButtons = [GotoProfile(), GotoSearch(), GotoProfile()]
        return FeedFloatingButtonLayout('-', actionButtons).layout()

    def refresh(self):
        for post in self.loaded_posts:
            self.posts_layout.remove_widget(post[1])
        self.load()

    def load(self):
        feed = self.get_feed()
        self.generate_posts(feed)

    def __init__(self, **kwargs):
        super(Feed, self).__init__(**kwargs)
        self.posts_layout = GridLayout(cols=1, spacing=20, size_hint_y=None)
        self.posts_layout.bind(minimum_height=self.posts_layout.setter('height'))
        self.posts_scroll = self.generate_root(self.posts_layout)

        mainWidget = FloatLayout()
        mainWidget.add_widget(self.posts_scroll)
        
        mainWidget.add_widget(self.generate_floating_button())

        self.add_widget(mainWidget)
        self.load()
        loading = Thread(target=self.load_posts)
        loading.start()


class Profile(Screen):
    username = StringProperty()
    subscribers = NumericProperty()
    subscriptions = NumericProperty()

    galleryLayout = None
    profileHeaded = None

    def get_posts(self, user_id):
        return server.get_posts(user_id)

    def generate_posts(self, posts):
        self.galleryLayout.bind(minimum_height=self.galleryLayout.setter('height'))
        for post in posts:
            loaded_post = PostMinimized()
            self.galleryLayout.add_widget(loaded_post)
            loaded_post.load(post)

    def generate_profile_header(self):
        return ProfileHeader().layout()

    def generate_gallery_root(self, galleryLayout):
        galleryRoot = ScrollView(size_hint=(1, None), size=(
            Window.width, Window.height - Window.width / 3 - Window.width / 8))
        galleryRoot.add_widget(galleryLayout)
        return galleryRoot

    def generate_floating_button(self):
        return ProfileFloatingButtonLayout('-', []).layout()

    def load(self, user_id):
        self.galleryLayout.clear_widgets()

        posts = self.get_posts(user_id)
        self.generate_posts(posts)

    def __init__(self, **kwargs):
        super(Profile, self).__init__(**kwargs)
        mainWidget = FloatLayout()

        profileLayout = BoxLayout(orientation='vertical')
        
        self.galleryLayout = GridLayout(cols=3, spacing=0, size_hint_y=None)

        self.profileHeaded = self.generate_profile_header()
        
        profileLayout.add_widget(GoBack())
        profileLayout.add_widget(self.profileHeaded)
        profileLayout.add_widget(self.generate_gallery_root(self.galleryLayout))

        mainWidget.add_widget(profileLayout)

        mainWidget.add_widget(self.generate_floating_button())

        self.add_widget(mainWidget)


class Search(Screen):
    galleryLayout = None
    searchField = None

    def get_posts(self, key):
        return server.search_post(key)

    def generate_posts(self, posts):
        self.galleryLayout.bind(minimum_height=self.galleryLayout.setter('height'))
        for post in posts:
            loaded_post = PostMinimized()
            self.galleryLayout.add_widget(loaded_post)
            loaded_post.load(post)

    def generate_gallery_root(self, galleryLayout):
        galleryRoot = ScrollView(size_hint=(1, None), size=(
            Window.width, Window.height - Window.width / 4))
        galleryRoot.add_widget(galleryLayout)
        return galleryRoot

    def generate_floating_button(self):
        return ProfileFloatingButtonLayout('-', []).layout()

    def load(self):
        self.galleryLayout.clear_widgets()
        print(self.searchField.search.text)
        posts = self.get_posts(
            self.searchField.text.text
        )
        self.generate_posts(posts)

    def p_load(self):
        loading = Thread(target=self.load)
        loading.start()

    def __init__(self, **kwargs):
        super(Search, self).__init__(**kwargs)
        mainWidget = FloatLayout()

        profileLayout = BoxLayout(orientation='vertical')
        
        self.galleryLayout = GridLayout(cols=3, spacing=0, size_hint_y=None)

        self.searchField = SearchField()
        
        profileLayout.add_widget(GoBack())
        profileLayout.add_widget(self.searchField)
        profileLayout.add_widget(self.generate_gallery_root(self.galleryLayout))

        mainWidget.add_widget(profileLayout)

        mainWidget.add_widget(self.generate_floating_button())

        self.add_widget(mainWidget)
        self.searchField.search.on_press = self.p_load
        #self.load()
        
    
from buttons import GotoButton, GotoProfile, GotoSearch, FeedFloatingButtonLayout, ProfileFloatingButtonLayout
from blocks import Post, ProfileHeader, Comment, CommentEditor, GoBack, PostMinimized, SearchField

feed = Feed(name='Feed')
openedPost = OpenedPost(name='OpenedPost')
openedUser = Profile(name='Profile')
screenController.opened_post = openedPost
screenController.opened_profile = openedUser
search = Search(name='Search')