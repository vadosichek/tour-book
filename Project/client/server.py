import urllib.request
import json


class Server():
    base_url = 'https://tour-book.herokuapp.com'

    def get(self, path):
        content = urllib.request.urlopen(self.base_url + path)
        return json.loads(content.read())

    def get_profile(self, user_id):
        return self.get('/get_profile/' + str(user_id))

    def get_feed(self, user_id):
        return self.get('/get_feed/' + str(user_id))

    def get_post(self, post_id):
        return self.get('/get_post/' + str(post_id))


server = Server()
