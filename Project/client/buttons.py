from kivy.uix.floatlayout import FloatLayout
from kivy.uix.button import Button
from kivy.properties import StringProperty
from kivy.properties import NumericProperty
from kivy.core.window import Window

from screens import Feed, OpenedPost, Profile, screenManager, screenController
from server import USER_ID

block = Window.width/5
margin = block/4

class HoverButton(Button):
    size_hint = (None, None)
    size = (block, block)
    x = NumericProperty(Window.width - block - margin)
    y = NumericProperty(margin)
    right = Window.width - margin
    top = block + margin

class GotoButton(HoverButton):

    def go(self):
        pass


class GotoProfile(GotoButton):

    def go(self):
        screenController.open_user(USER_ID)
    on_press = go

class GotoSearch(GotoButton):

    def go(self):
        screenController.open_search()
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
        y = (block * 3)/2
        for actionButton in actionButtons:
            actionButton.x = Window.width - block - margin
            actionButton.y = y
            actionButton.top = block + y
            actionButtonsBlockLayout.add_widget(actionButton)
            y += block + margin
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

class SearchFloatingButtonLayout(FloatingButtonLayout):
    state = False

    def load(self, posts, users, search):
        self.posts = posts
        self.users = users
        self.search = search

    def open(self):
        self.search.remove_widget(self.posts)
        self.search.add_widget(self.users)

