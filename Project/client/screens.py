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

    def layout(self):
        layout = GridLayout(cols=1, spacing=10, size_hint_y=None)
        layout.bind(minimum_height=layout.setter('height'))
        layout.add_widget(
            Post().layout(USER_ID, "username", "desc", 4, 9 / 4))
        mainWidget = ScrollView(size_hint=(
            1, None), size=(Window.width, Window.height))
        mainWidget.add_widget(layout)
        return mainWidget


class Feed(Screen):
    def layout(self):
        layout = GridLayout(cols=1, spacing=10, size_hint_y=None)
        layout.bind(minimum_height=layout.setter('height'))
        feed = server.get_feed(USER_ID)
        for post in feed:
            data = server.get_post(post)
            layout.add_widget(
                Post().layout(data['id'], data['name'], data['description'], 1, 1.5))
        root = ScrollView(size_hint=(1, None), size=(
            Window.width, Window.height))
        root.add_widget(layout)

        mainWidget = FloatLayout()
        mainWidget.add_widget(root)
        actionButtons = [GotoProfile(), GotoProfile(), GotoProfile()]
        floatingButton = FeedFloatingButtonLayout('-', actionButtons).layout()
        mainWidget.add_widget(floatingButton)

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

    def layout(self, user_id):
        mainWidget = FloatLayout()

        profileLayout = BoxLayout(orientation='vertical')

        posts = self.get_posts(user_id)
        galleryLayout = self.generate_posts(posts)

        profileLayout.add_widget(self.generate_profile_header())
        profileLayout.add_widget(self.generate_gallery_root(galleryLayout))

        mainWidget.add_widget(profileLayout)

        floatingButton = ProfileFloatingButtonLayout('-', []).layout()
        mainWidget.add_widget(floatingButton)

        return mainWidget

    
from buttons import GotoButton, GotoProfile, FeedFloatingButtonLayout, ProfileFloatingButtonLayout
from blocks import Post, ProfileHeader
