from kivy.uix.floatlayout import FloatLayout
from kivy.uix.boxlayout import BoxLayout
from kivy.uix.button import Button
from kivy.uix.label import Label
from kivy.properties import StringProperty
from kivy.core.window import Window
from screens import screenController, OpenedPost


class Post(BoxLayout):

    def openPost(self):
        screenController.setCurrentScreen(OpenedPost().layout())

    def layout(self, username, description, commentsSize, size):
        layout = BoxLayout()
        layout.size = (Window.width, Window.width * size)
        layout.size_hint_y = None
        layout.orientation = 'vertical'
        header = BoxLayout(
            orientation='horizontal',
            size_hint_y=0.5)
        header.add_widget(
            Button(
                text='usr',
                size_hint_x=0.5))
        header.add_widget(
            Label(
                text=username,
                halign='left',
                valign='middle',
                size_hint_x=3,
                font_size=self.width / 4))
        header.add_widget(
            Button(
                text='-',
                size_hint_x=0.5))
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
                size_hint_x=2))
        interaction.add_widget(
            Button(
                text='comment',
                size_hint_x=2))
        layout.add_widget(interaction)
        comments = Button(size_hint_y=commentsSize, size_hint_x=4)
        comments.on_press = self.openPost
        layout.add_widget(comments)
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
