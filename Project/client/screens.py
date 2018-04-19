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


class ScreenController():

    def __init__(self):
        self.currentScreen = FloatLayout()

    def getCurrentScreen(self):
        return self.currentScreen

    def setCurrentScreen(self, newScreen):
        self.currentScreen.clear_widgets()
        self.currentScreen.add_widget(newScreen)

screenController = ScreenController()


class Screen():

    def layout(self):
        return None


class OpenedPost(Screen):

    def layout(self, post, user_id, username, description, likes_count, comments_count):
        layout = GridLayout(cols=1, spacing=20, size_hint_y=None)
        layout.bind(minimum_height=layout.setter('height'))
        layout.add_widget(
            Post().layout(post, user_id, username, description, likes_count, comments_count, 5/4))

        comments = server.get_comments(post)
        print(comments)
        for comment in comments:
            layout.add_widget(Comment().layout(comment['user_name'], comment['text']))
        
        mainWidget = ScrollView(size_hint=(
            1, None), size=(Window.width, Window.height))
        mainWidget.add_widget(layout)
        return mainWidget


class Feed(Screen):

    def get_feed(self):
        return server.get_feed(USER_ID)

    def generate_posts(self, feed):
        layout = GridLayout(cols=1, spacing=10, size_hint_y=None)
        layout.bind(minimum_height=layout.setter('height'))
        print(feed)
        for post in feed:
            data = server.get_post(post)
            comments = server.get_comments(post)
            print(post)
            print(data)
            print(len(comments))
            layout.add_widget(
                Post().layout(post, data['id'], data['name'], data['description'], 0, len(comments), 5/4))
        return layout

    def generate_root(self, layout):
        root = ScrollView(size_hint=(1, None), size=(
            Window.width, Window.height))
        root.add_widget(layout)
        return root

    def generate_floating_button(self):
        actionButtons = [GotoProfile(), GotoProfile(), GotoProfile()]
        return FeedFloatingButtonLayout('-', actionButtons).layout()

    def layout(self):
        feed = self.get_feed()
        layout = self.generate_posts(feed)
        root = self.generate_root(layout)

        mainWidget = FloatLayout()
        mainWidget.add_widget(root)
        
        mainWidget.add_widget(self.generate_floating_button())

        return mainWidget


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
from blocks import Post, ProfileHeader, Comment
