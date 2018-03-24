import kivy
# kivy.require('1.9.0')
from kivy.config import Config
# Config.set('graphics', 'resizable', 0)
Config.set('graphics', 'width', '450')
Config.set('graphics', 'height', '800')


from kivy.app import App
from kivy.uix.gridlayout import GridLayout
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.scrollview import ScrollView
from kivy.uix.floatlayout import FloatLayout
from kivy.uix.button import Button
from kivy.uix.label import Label
from kivy.properties import StringProperty
from kivy.properties import ObjectProperty
from kivy.properties import NumericProperty
from kivy.core.window import Window


class Post(BoxLayout):
    username = StringProperty()
    description = StringProperty()

    def openPost(self):
        screenController.setCurrentScreen(OpenedPost().layout())


class PostWithComments(BoxLayout):
    username = StringProperty()
    description = StringProperty()


class HoverButton(Button):
    text = StringProperty()
    size_hint = (None, None)
    size = (100, 100)
    x = NumericProperty(Window.width - 125)
    y = NumericProperty(40)


class GotoButton(HoverButton):

    def go(self):
        pass


class GotoProfile(GotoButton):

    def go(self):
        screenController.setCurrentScreen(Profile().layout())
    on_press = go


class FloatingButtonLayout():
    actionButtons = None
    actionButtonsLayout = None
    floatingButton = None
    showActionButtons = False

    def open(self):
        pass

    def generateActionButtonsLayout(self, actionButtons):
        actionButtonsBlockLayout = FloatLayout()
        y = 150
        for actionButton in actionButtons:
            actionButton.x = Window.width - 125
            actionButton.y = y
            actionButtonsBlockLayout.add_widget(actionButton)
            y += 110
        return actionButtonsBlockLayout

    def __init__(self, floatingButtonText, actionButtons):
        self.actionButtons = self.generateActionButtonsLayout(actionButtons)
        self.actionButtonsLayout = FloatLayout()
        self.floatingButton = HoverButton()
        self.floatingButton.text = floatingButtonText
        self.floatingButton.on_press = self.open

    def layout(self):
        root = FloatLayout()
        root.add_widget(self.floatingButton)
        root.add_widget(self.actionButtonsLayout)
        return root


class FeedFloatingButtonLayout(FloatingButtonLayout):

    def open(self):
        if self.showActionButtons:
            self.showActionButtons = False
            self.actionButtonsLayout.clear_widgets()
        else:
            self.showActionButtons = True
            self.actionButtonsLayout.add_widget(self.actionButtons)


class ProfileFloatingButtonLayout(FloatingButtonLayout):

    def open(self):
        # subscribe
        pass


class Screen():

    def layout(self):
        return None


class Feed(Screen):

    def layout(self):
        layout = GridLayout(cols=1, spacing=10, size_hint_y=None)
        layout.bind(minimum_height=layout.setter('height'))
        for i in range(100):
            layout.add_widget(
                Post(username="username " + str(i), description="desc", size=(Window.width, Window.width * 1.5),
                     size_hint_y=None))
        root = ScrollView(size_hint=(1, None), size=(
            Window.width, Window.height))
        root.add_widget(layout)

        mainWidget = FloatLayout()
        mainWidget.add_widget(root)
        actionButtons = [GotoProfile(), GotoProfile(), GotoProfile()]
        floatingButton = FeedFloatingButtonLayout('-', actionButtons).layout()
        mainWidget.add_widget(floatingButton)

        return mainWidget


class OpenedPost(Screen):

    def layout(self):
        layout = GridLayout(cols=1, spacing=10, size_hint_y=None)
        layout.bind(minimum_height=layout.setter('height'))
        layout.add_widget(
            PostWithComments(username="username ", description="desc", size=(Window.width, Window.width * 9 / 4),
                             size_hint_y=None))
        mainWidget = ScrollView(size_hint=(
            1, None), size=(Window.width, Window.height))
        mainWidget.add_widget(layout)
        return mainWidget


class ProfileHeader(BoxLayout):
    pass


class Profile(Screen):
    username = StringProperty()
    subscribers = NumericProperty()
    subscriptions = NumericProperty()

    def layout(self):
        mainWidget = FloatLayout()
        profileLayout = BoxLayout(orientation='vertical')
        profileHeader = ProfileHeader(
            size=(Window.width, Window.width / 3), size_hint=(None, None))

        galleryLayout = GridLayout(cols=3, spacing=0, size_hint_y=None)
        galleryLayout.bind(minimum_height=galleryLayout.setter('height'))
        for i in range(100):
            galleryLayout.add_widget(Button(text='img', size_hint=(
                None, None), size=(Window.width / 3, Window.width / 3)))
        galleryRoot = ScrollView(size_hint=(1, None), size=(
            Window.width, Window.height - Window.width / 3))
        galleryRoot.add_widget(galleryLayout)
        profileLayout.add_widget(profileHeader)
        profileLayout.add_widget(galleryRoot)

        mainWidget.add_widget(profileLayout)
        actionButtons = []
        floatingButton = ProfileFloatingButtonLayout('-', []).layout()
        mainWidget.add_widget(floatingButton)

        return mainWidget


class ScreenController():

    def __init__(self):
        self.currentScreen = FloatLayout()

    def getCurrentScreen(self):
        return self.currentScreen

    def setCurrentScreen(self, newScreen):
        self.currentScreen.clear_widgets()
        self.currentScreen.add_widget(newScreen)

screenController = ScreenController()


class PanoramaApp(App):

    def build(self):
        screenController.setCurrentScreen(Feed().layout())
        return screenController.getCurrentScreen()


if __name__ == '__main__':
    PanoramaApp().run()
