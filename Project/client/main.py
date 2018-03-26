import kivy
import config

from kivy.app import App
from screens import Feed, OpenedPost, Profile, screenController


class PanoramaApp(App):

    def build(self):
        screenController.setCurrentScreen(Feed().layout())
        return screenController.getCurrentScreen()


if __name__ == '__main__':
    PanoramaApp().run()
