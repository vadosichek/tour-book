import kivy
import config

from kivy.app import App
from kivy.core.window import Window
from screens import Feed, OpenedPost, Profile, screenManager, screenController
from screens import feed, openedPost, openedUser

screenManager.add_widget(feed)
screenManager.add_widget(openedPost)
screenManager.add_widget(openedUser)
screenController.save_last('Feed')

class PanoramaApp(App):
    def build(self):
        screenManager.current = 'Feed'
        return screenManager

if __name__ == '__main__':
    PanoramaApp().run()
    