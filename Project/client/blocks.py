from kivy.clock import mainthread
from kivy.uix.floatlayout import FloatLayout
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.gridlayout import GridLayout
from kivy.uix.button import Button
from kivy.uix.label import Label
from kivy.uix.textinput import TextInput
from kivy.properties import StringProperty
from kivy.core.window import Window
from screens import screenManager, screenController, OpenedPost, Profile
from server import server

class Post(BoxLayout):
    likes_label = None
    comments_label = None
    name_label = None
    post = None
    user = None

    def open_post(self):
        screenController.open_post(self)

    def open_user(self):
        screenController.open_user(self.user)

    def copy(self, post):
        self.likes_label.text = post.likes_label.text
        self.comments_label.text = post.comments_label.text
        self.name_label.text = post.name_label.text
        self.user = post.user

    def load(self, post):
        self.post = post
        data = server.get_post(self.post)
        self.likes_label.text = str(data['likes'])
        self.comments_label.text = str(data['comments'])
        self.name_label.text = data['name']
        self.user = data['id']

    def __init__(self, **kwargs):
        super(Post, self).__init__(**kwargs)
        self.size_hint_y = None
        self.size = (Window.width, Window.width * 5/4)
        self.orientation = 'vertical'
        header = BoxLayout(
            orientation='horizontal',
            size_hint_y=0.5)
        user_button = Button(
            text='usr',
            size_hint_x=0.5)
        user_button.on_press = self.open_user
        header.add_widget(user_button)
        self.name_label = Label(
                text='',
                halign='left',
                valign='top',
                size_hint_x=3.5,
                text_size=(Window.width/1.2, None),
                font_size=header.height)
        header.add_widget(self.name_label)
        self.add_widget(header)
        self.add_widget(
            Button(
                size_hint_y=4,
                size_hint_x=4))
        interaction = BoxLayout(
            orientation='horizontal',
            size_hint_y=0.5)
        like = Button(
                text='like',
                size_hint_x=0.5)
        like_callback = lambda:server.create_like(server.get_user_id(), self.post)
        like.on_press = like_callback
        interaction.add_widget(like)
        self.likes_label = Label(
                text='',
                size_hint_x=0.5)
        interaction.add_widget(self.likes_label)
        interaction.add_widget(
            Label(
                text='',
                size_hint_x=2))
        comments = Button(
                text='comment',
                size_hint_x=0.5)
        comments.on_press = self.open_post
        interaction.add_widget(comments)
        self.comments_label = Label(
                text='',
                size_hint_x=0.5)
        interaction.add_widget(self.comments_label)
        self.add_widget(interaction)
        


class ProfileHeader(BoxLayout):

    def layout(self):
        layout = BoxLayout()
        layout.orientation = 'horizontal'
        layout.size = (Window.width, Window.width / 3)
        layout.size_hint = (None, None)
        layout.add_widget(
            Button(
                text='userpic',
                size_hint=(1, 1)))
        data = BoxLayout(
            size_hint=(2, 1),
            orientation='vertical')
        data.add_widget(
            Button(
                text='1111',
                size_hint=(1, 1)))
        data.add_widget(
            Button(
                text='2222',
                size_hint=(1, 1)))
        data.add_widget(
            Button(
                text='3333',
                size_hint=(1, 1)))
        layout.add_widget(data)
        return layout

class Comment(Label):
    def __init__(self, **kwargs):
        super(Comment, self).__init__(**kwargs)
        self.markup = True
        self.size_hint_y=None
        self.text_size=(Window.width/1.2, None)
        self.halign='left'

    @mainthread
    def upd_texture(self):
        self.texture_update()
        self.height=self.texture_size[1] 
        self.texture_update()

    def load(self, user, text):
        self.text="[b]{0}:[/b] {1}".format(user, text)
        self.upd_texture()

class CommentEditor():

    def layout(self, post):
        layout = BoxLayout()
        layout.size_hint_y = None
        layout.size = (Window.width, Window.width / 8)
        layout.orientation = 'horizontal'

        text = TextInput(text="", size_hint_x=.875)
        send = Button(text="send", size_hint_x=.125)

        def refresh():
            server.create_comment(server.get_user_id(), post, text.text)

        send.on_press = refresh

        layout.add_widget(text)
        layout.add_widget(send)

        return layout

class GoBack(Button):

    def press_callback(self):
        screenController.go_back()

    def __init__(self, **kwargs):
        super(GoBack, self).__init__(**kwargs)
        self.size_hint_y = None
        self.size = (Window.width, Window.width / 8)
        self.on_press = self.press_callback

class PostMinimized(Button):
    post = None

    def load(self, post):
        self.post = post
        self.text = str(post['id'])

    def open(self):
        screenController.open_post(self.post['id'])

    def __init__(self, **kwargs):
        super(PostMinimized, self).__init__(**kwargs)
        self.on_press = self.open
        self.size = (Window.width / 3, Window.width / 3)
        self.size_hint = (None, None)

class SearchField(BoxLayout):
    text = None
    search = None
    def __init__(self, **kwargs):
        super(SearchField, self).__init__(**kwargs)
        self.size_hint_y = None
        self.size = (Window.width, Window.width / 8)
        self.orientation = 'horizontal'

        self.text = TextInput(text="", size_hint_x=.875)
        self.search = Button(text="search", size_hint_x=.125)

        self.add_widget(self.text)
        self.add_widget(self.search)

class UserMinimized(BoxLayout):

    def open(self):
        screenController.open_user(self.user)

    def load(self, user):
        self.user = int(user['id'])
        self.login.text = user['login']
        self.name.text = user['name']

    def __init__(self, **kwargs):
        super(UserMinimized, self).__init__(**kwargs)
        self.size_hint_y = None
        self.size = (Window.width, Window.width / 8)
        self.orientation = 'horizontal'

        self.pic = Button(text='', size_hint_x=.125)
        self.pic.on_press = self.open

        labels = BoxLayout(orientation='vertical', size_hint_x=.875)
        self.login = Label(text='')
        self.name = Label(text='')

        labels.add_widget(self.login)
        labels.add_widget(self.name)

        self.add_widget(self.pic)
        self.add_widget(labels)

class LoginMenu(GridLayout):

    def login(self):
        responce = server.login(self.login_field.text, self.password_field.text)
        server.update_user(responce)
        if not responce == -1:
            screenController.open_feed()
        
        print(server.get_user_id())

    def __init__(self, **kwargs):
        super(LoginMenu, self).__init__(**kwargs)
        self.size_hint_y = None
        self.size_hint_x = None
        self.size = (Window.width/8 * 7, Window.height/4)
        self.cols = 1
        self.spacing = 20
        self.pos_hint = {'center_y':.8, 'center_x':.5}
        
        self.add_widget(Label(text='Circum'))

        self.login_field = TextInput(text='Login')
        self.add_widget(self.login_field)

        self.password_field = TextInput(text='Password')
        self.add_widget(self.password_field)    

        self.login_button = Button(text='Login')
        self.login_button.on_press = self.login
        self.add_widget(self.login_button)