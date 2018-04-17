import urllib.request
import json
USER_ID = 372

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

    def get_comments(self, tour_id):
        return self.get('/get_comments/' + str(post_id))

    def get_posts(self, user_id):
        return self.get('/get_posts/' + str(user_id))

    def create_profile(self, login, password, name, bio, url, pic):
        return self.get(
            '/create_profile?login={0}&password={1}&name={2}&bio={3}&url={4}&pic={5}'.format(
            login, password, name, bio, url, pic))

    def create_tour(self, path, user_id, geotag, desc, tags, size, time, pic):
        return self.get(
            '/create_tour?path={0}&user_id={1}&geotag={2}&desc={3}&tags={4}&size={5}&time={6}&pic={7}'.format(
            path, user_id, geotag, desc, tags, size, time, pic))

    def create_comment(self, user_id, tour_id, text):
        return self.get(
            '/create_comment?user_id={0}&tour_id={1}&text={2}'.format(
            user_id, tour_id, text))

    def create_like(self, user_id, tour_id):
        return self.get(
            '/create_like?user_id={0}&tour_id={1}'.format(
            user_id, tour_id))

    def create_subscription(self, user_id, subscriber_id):
        return self.get(
            '/create_subscription?user_id={0}&subscriber_id={1}'.format(
            user_id, subscriber_id))

server = Server()
