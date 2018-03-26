from kivy.uix.floatlayout import FloatLayout
from kivy.uix.button import Button
from kivy.properties import StringProperty
from kivy.properties import NumericProperty
from kivy.core.window import Window

from screens import Feed, OpenedPost, Profile, screenController


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
