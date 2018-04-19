from kivy.uix.floatlayout import FloatLayout
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.button import Button
from kivy.uix.label import Label
from kivy.properties import StringProperty
from kivy.core.window import Window
from screens import screenController, OpenedPost, Profile


class Post(BoxLayout):

    def open_post(self, post, user_id, username, description, likes_count, comments_count):
        screenController.setCurrentScreen(OpenedPost().layout(post, user_id, username, description, likes_count, comments_count))

    def open_user(self, user_id):
        screenController.setCurrentScreen(Profile().layout(user_id))

    def layout(self, post, user_id, username, description, likes_count, comments_count, size):
        layout = BoxLayout()
        layout.size = (Window.width, Window.width * size)
        layout.size_hint_y = None
        layout.orientation = 'vertical'
        header = BoxLayout(
            orientation='horizontal',
            size_hint_y=0.5)
        user_button = Button(
            text='usr',
            size_hint_x=0.5)
        user_button_callback = lambda:self.open_user(user_id)
        user_button.on_press = user_button_callback
        header.add_widget(user_button)
        header.add_widget(
            Label(
                text=username,
                halign='left',
                valign='top',
                size_hint_x=3.5,
                text_size=(Window.width/1.2, None)))
        layout.add_widget(header)
        layout.add_widget(
            Button(
                size_hint_y=4,
                size_hint_x=4))
        interaction = BoxLayout(
            orientation='horizontal',
            size_hint_y=0.5)
        interaction.add_widget(
            Button(
                text='like',
                size_hint_x=0.5))
        interaction.add_widget(
            Label(
                text='0',
                size_hint_x=0.5))
        interaction.add_widget(
            Label(
                text='',
                size_hint_x=2))
        comments = Button(
                text='comment',
                size_hint_x=0.5)
        comments_callback = lambda:self.open_post(post, user_id, username, description, likes_count, comments_count)
        comments.on_press = comments_callback
        interaction.add_widget(comments)
        interaction.add_widget(
            Label(
                text=str(comments_count),
                size_hint_x=0.5))
        layout.add_widget(interaction)
        return layout


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
        return Label(text="[b]{0}:[/b] {1}".format(user, text), markup=True, text_size=(Window.width, None), halign='left')