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

from kivy.uix.screenmanager import ScreenManager, Screen

class ScreenController():

    def __init__(self):
        self.currentScreen = FloatLayout()

    def getCurrentScreen(self):
        return self.currentScreen

    def setCurrentScreen(self, newScreen):
        self.currentScreen.clear_widgets()
        self.currentScreen.add_widget(newScreen)

#screenController = ScreenController()
screenManager = ScreenManager()

# class Screen():

#     def layout(self):
#         return None


class OpenedPost(Screen):

    def generate_post(self, post, user_id, username, description, likes_count, comments_count):
        return Post().layout(post, user_id, username, description, likes_count, comments_count)

    def generate_comment_editor(self, post, user_id, username, description, likes_count):
        return CommentEditor().layout(post, user_id, username, description, likes_count)

    def generate_comments(self, post):
        return server.get_comments(post)

    def __init__(self, post, user_id, username, description, likes_count):
        #super(OpenedPost, self).__init__(**kwargs)
        layout = GridLayout(cols=1, spacing=0, size_hint_y=None)
        layout.bind(minimum_height=layout.setter('height'))
        comments = self.generate_comments(post)
        layout.add_widget(self.generate_post(post, user_id, username, description, likes_count, len(comments)))
        
        layout.add_widget(self.generate_comment_editor(post, user_id, username, description, likes_count))
        
        for comment in comments:
            layout.add_widget(Comment().layout(comment['user_name'], comment['text']))

        mainWidget = ScrollView(size_hint=(
            1, None), size=(Window.width, Window.height))
        mainWidget.add_widget(layout)
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

    def load_posts(self, pd):
        for post in self.loaded_posts:
            post[1].load(post[0])

    def generate_root(self, layout):
        root = ScrollView(size_hint=(1, None), size=(
            Window.width, Window.height))
        root.add_widget(layout)
        return root

    def generate_floating_button(self):
        actionButtons = [GotoProfile(), GotoProfile(), GotoProfile()]
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
        Clock.schedule_once(self.load_posts, 1.5)
        #self.load_posts()


class Profile(Screen):
    username = StringProperty()
    subscribers = NumericProperty()
    subscriptions = NumericProperty()

    def get_posts(self, user_id):
        return server.get_posts(user_id)

    def generate_posts(self, posts):
        galleryLayout = GridLayout(cols=3, spacing=0, size_hint_y=None)
        galleryLayout.bind(minimum_height=galleryLayout.setter('height'))
        for post in posts:
            galleryLayout.add_widget(Button(text='img', size_hint=(
                None, None), size=(Window.width / 3, Window.width / 3)))
        return galleryLayout

    def generate_profile_header(self):
        return ProfileHeader().layout()

    def generate_gallery_root(self, galleryLayout):
        galleryRoot = ScrollView(size_hint=(1, None), size=(
            Window.width, Window.height - Window.width / 3))
        galleryRoot.add_widget(galleryLayout)
        return galleryRoot

    def generate_floating_button(self):
        return ProfileFloatingButtonLayout('-', []).layout()

    def layout(self, user_id):
        mainWidget = FloatLayout()

        profileLayout = BoxLayout(orientation='vertical')

        posts = self.get_posts(user_id)
        galleryLayout = self.generate_posts(posts)

        profileLayout.add_widget(self.generate_profile_header())
        profileLayout.add_widget(self.generate_gallery_root(galleryLayout))

        mainWidget.add_widget(profileLayout)

        mainWidget.add_widget(self.generate_floating_button())

        return mainWidget

    
from buttons import GotoButton, GotoProfile, FeedFloatingButtonLayout, ProfileFloatingButtonLayout
from blocks import Post, ProfileHeader, Comment, CommentEditor

feed = Feed(name='Feed')
#openedPost = OpenedPost(name='OpenedPost')