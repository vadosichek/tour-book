from kivy.clock import Clock
from kivy.uix.floatlayout import FloatLayout
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.button import Button
from kivy.uix.label import Label
from kivy.uix.textinput import TextInput
from kivy.properties import StringProperty
from kivy.core.window import Window
from screens import screenManager, OpenedPost, Profile
from server import server, USER_ID

class Post(BoxLayout):
    likes_label = None
    comments_label = None
    name_label = None
    post = None

    def open_post(self, post):
        screenManager.current = 'OpenedPost'

    def open_user(self, user_id):
        screenController.setCurrentScreen(Profile().layout(user_id))

    def load(self, post):
        self.post = post
        data = server.get_post(self.post)
        comments = server.get_comments(self.post)
        likes = server.get_likes(self.post)
        self.likes_label.text = str(len(likes))
        self.comments_label.text = str(len(comments))
        self.name_label.text = data['name']

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
        user_button_callback = lambda:self.open_user(user_id)
        user_button.on_press = user_button_callback
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
        like_callback = lambda:server.create_like(USER_ID, post)
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
        comments_callback = lambda:self.open_post(post, user_id, username, description, likes_count)
        comments.on_press = comments_callback
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

class Comment():

    def layout(self, user, text):
        comment_label = Label(text="[b]{0}:[/b] {1}".format(user, text), markup=True, size_hint_y=None, text_size=(Window.width/1.2, None), halign='left')
        comment_label.texture_update()
        comment_label.height=comment_label.texture_size[1] 
        comment_label.texture_update()
        print(comment_label.texture_size)
        return comment_label

class CommentEditor():

    def layout(self, post, user_id, username, description, likes_count):
        layout = BoxLayout()
        layout.size_hint_y = None
        layout.size = (Window.width, Window.width / 8)
        layout.orientation = 'horizontal'

        text = TextInput(text="", size_hint_x=.875)
        send = Button(text="send", size_hint_x=.125)

        def refresh():
            server.create_comment(USER_ID, post, text.text)
            screenController.setCurrentScreen(OpenedPost().layout(post, user_id, username, description, likes_count))

        send.on_press = refresh

        layout.add_widget(text)
        layout.add_widget(send)

        return layout