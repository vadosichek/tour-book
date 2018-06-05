import kivy
import config

from kivy.app import App
from kivy.core.window import Window
from screens import Feed, OpenedPost, Profile, screenManager, screenController
from screens import feed, openedPost, openedUser, search, login, registrate
import json
from server import server
from crypt import decrypt

screenManager.add_widget(feed)
screenManager.add_widget(openedPost)
screenManager.add_widget(openedUser)
screenManager.add_widget(search)
screenManager.add_widget(login)
screenManager.add_widget(registrate)

def check_auth():
        with open('data.json', 'r') as infile:
                data = infile.read()
                try:
                    json_data = json.loads(data)
                    responce = server.login(json_data['login'], json_data['password'])
                    if not responce == -1:
                        screenController.open_feed()
                        return False
                except ValueError:
                    return True
        return True

class PanoramaApp(App):
    def build(self):
        if check_auth():
            screenManager.current = 'Login'  
        return screenManager

if __name__ == '__main__':
    PanoramaApp().run()
    