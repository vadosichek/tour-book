from kivy.uix.boxlayout import BoxLayout
from kivy.properties import StringProperty


class Post(BoxLayout):
    username = StringProperty()
    description = StringProperty()

    def openPost(self):
        screenController.setCurrentScreen(OpenedPost().layout())


class ProfileHeader(BoxLayout):
    pass


class PostWithComments(BoxLayout):
    username = StringProperty()
    description = StringProperty()
