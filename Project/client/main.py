import kivy
import config

from kivy.app import App
from screens import Feed, OpenedPost, Profile, screenManager
from screens import feed

screenManager.add_widget(feed)
#screenManager.add_widget(openedPost)

class PanoramaApp(App):

    def build(self):
        screenManager.current = 'Feed'
        return screenManager

if __name__ == '__main__':
    PanoramaApp().run()
    