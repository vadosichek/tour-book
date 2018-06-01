import kivy
import config

from kivy.app import App
from kivy.core.window import Window
from screens import Feed, OpenedPost, Profile, screenManager, screenController
from screens import feed, openedPost, openedUser, search, login, registrate

screenManager.add_widget(feed)
screenManager.add_widget(openedPost)
screenManager.add_widget(openedUser)
screenManager.add_widget(search)
screenManager.add_widget(login)
screenManager.add_widget(registrate)
screenController.save_last('Login')

class PanoramaApp(App):
    def build(self):
        screenManager.current = 'Login'    
        return screenManager

if __name__ == '__main__':
    PanoramaApp().run()
    