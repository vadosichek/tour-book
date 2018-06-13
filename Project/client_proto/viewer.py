from kivy.uix.floatlayout import FloatLayout
from kivy3 import Mesh, Material
from kivy3 import Scene, Renderer, PerspectiveCamera
from kivy3.extras.geometries import BoxGeometry
from kivy3.loaders import OBJLoader
from kivy.uix.screenmanager import Screen
import math


class ObjectTrackball(FloatLayout):

    def __init__(self, camera, obj, radius, *args, **kw):
        super(ObjectTrackball, self).__init__(*args, **kw)
        self.camera = camera
        self.radius = radius
        self.phi = 90
        self.theta = 0
        self._touches = []
        self.camera.pos = (0,0,0)
        self.camera.look_at((20, 0, 0))
        self.obj = obj

    def define_rotate_angle(self, touch):
        theta_angle = (touch.dx / self.width) * -360
        phi_angle = -1 * (touch.dy / self.height) * 360
        return phi_angle, theta_angle

    def on_touch_down(self, touch):
        touch.grab(self)
        self._touches.append(touch)

    def on_touch_up(self, touch):
        touch.ungrab(self)
        self._touches.remove(touch)

    def on_touch_move(self, touch):
        if touch in self._touches and touch.grab_current == self:
            if len(self._touches) == 1:
                self.do_rotate(touch)
            elif len(self._touches) == 2:
                pass

    def do_rotate(self, touch):
        d_phi, d_theta = self.define_rotate_angle(touch)
        self.phi += d_phi
        self.theta += d_theta

        _phi = math.radians(self.phi)
        _theta = math.radians(self.theta)
        x = self.radius * _theta
        y = self.radius * _phi
        self.camera.look_at((y, 0, 0))
        #self.obj.rotation.x = -y
        self.camera.rotation.y = -x

obj_file = "cube.obj"

class Viewer(Screen):

    def __init__(self, **kwargs):
        super(Viewer, self).__init__(**kwargs)
        print('loaded')
        self.renderer = Renderer()
        scene = Scene()

        # for i in range(3):
        #     geometry = BoxGeometry(10, 2, 1)
        #     material = Material(color=(0., 0., 1.), diffuse=(1., 1., 0.),
        #                         specular=(.35, .35, .35))
        #     self.cube = Mesh(geometry, material)
        #     self.cube.pos.z = 5*i
        #     self.cube.pos.x = 50
        #     scene.add(self.cube)

        camera = PerspectiveCamera(15, 0.3, 1, 1000)


        loader = OBJLoader()
        obj = loader.load(obj_file)
        self.obj3d = obj
        self.obj3d.pos.x = 1000
        self.obj3d.rotation.y = 90
        self.obj3d.size = 1, 1, 1
        scene.add(self.obj3d)


        self.camera = camera
        root = ObjectTrackball(camera, self.obj3d, 5)

        

        self.renderer.render(scene, camera)

        root.add_widget(self.renderer)
        self.renderer.bind(size=self._adjust_aspect)
        self.add_widget(root)

    def _adjust_aspect(self, inst, val):
        rsize = self.renderer.size
        aspect = rsize[0] / float(rsize[1])
        self.renderer.camera.aspect = aspect