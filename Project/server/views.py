from run import app, db
import models
from flask import request, send_from_directory, redirect, url_for
import json
from datetime import date, datetime
from werkzeug.security import generate_password_hash, check_password_hash
from werkzeug.utils import secure_filename
import os
import fnmatch
from PIL import Image
#from crypt import decrypt

def check_password(user_id, password):
    user = models.User.query.get(user_id)
    if user != None:
        correct = check_password_hash(user.password, password)
        return user.id if correct else -1
    return -1

@app.route('/get_panorama', methods=['GET', 'POST'])
def get_panorama():
    post = request.args.get('id')
    name = request.args.get('name')

    for file in os.listdir(os.path.join(app.config['UPLOAD_FOLDER'], 'posts', str(post), 'panoramas')):
        if fnmatch.fnmatch(file, str(name)+'.*'):
            return send_from_directory(os.path.join(app.config['UPLOAD_FOLDER'], 'posts', str(post), 'panoramas'),
                               file)

    return 'err'

@app.route('/get_photo', methods=['GET', 'POST'])
def get_photo():
    post = request.args.get('id')
    name = request.args.get('name')

    for file in os.listdir(os.path.join(app.config['UPLOAD_FOLDER'], 'posts', str(post), 'photos')):
        if fnmatch.fnmatch(file, str(name)+'.*'):
            return send_from_directory(os.path.join(app.config['UPLOAD_FOLDER'], 'posts', str(post), 'photos'),
                               file)

    return 'err'

@app.route('/get_user', methods=['GET', 'POST'])
def get_user():
    name = request.args.get('name')

    for file in os.listdir(os.path.join(app.config['UPLOAD_FOLDER'], 'users')):
        if fnmatch.fnmatch(file, str(name)+'.*'):
            return send_from_directory(os.path.join(app.config['UPLOAD_FOLDER'], 'users'),
                               file)

    return 'err'

@app.route('/get_tour', methods=['GET', 'POST'])
def get_tour():
    id = request.args.get('tour')

    path_base = os.path.join(app.config['UPLOAD_FOLDER'], 'posts', id, 'tour.json')

    text = ''
    with open(path_base, 'r') as f:
        text = f.read()

    return text

html_form = '''
            <!doctype html>
            <title>Upload new File</title>
            <h1>Upload new File</h1>
            <form method=post enctype=multipart/form-data>
              <input type=file name=file>
              <input type=text name=tour>
              <input type=submit value=Upload>
            </form>
            '''

@app.route('/upload_panorama', methods=['GET', 'POST'])
def upload_panorama():
    if request.method == 'POST':
        file = request.files['file']
        return upload_for_posts(file, 'panoramas', request.form.get('tour'))
    return html_form

@app.route('/upload_photo', methods=['GET', 'POST'])
def upload_photo():
    if request.method == 'POST':
        file = request.files['file']
        return upload_for_posts(file, 'photos', request.form.get('tour'))
    return html_form

def create_thumb(path_photo, filename, size=500):
    image = Image.open(os.path.join(path_photo, filename))

    x = image.size[0]
    y = image.size[1]
    axis = min(x, y)

    box = (x/2 - axis/2, y/2 - axis/2, x/2 + axis/2, y/2 + axis/2)
    image = image.crop(box)

    image = image.resize((size, size))

    image.save(os.path.join(path_photo, 'thumb_'+filename))


@app.route('/create_thumb_panorama', methods=['GET', 'POST'])
def create_thumb_panorama():
    post = request.args.get('id')

    for file in os.listdir(os.path.join(app.config['UPLOAD_FOLDER'], 'posts', str(post), 'panoramas')):
        create_thumb(os.path.join(app.config['UPLOAD_FOLDER'], 'posts', str(post), 'panoramas'), file)

    return 'done'

@app.route('/create_thumb_user', methods=['GET', 'POST'])
def create_thumb_user():

    for file in os.listdir(os.path.join(app.config['UPLOAD_FOLDER'], 'users')):
        create_thumb(os.path.join(app.config['UPLOAD_FOLDER'], 'users'), file, 200)

    return 'done'

def upload_for_posts(file, opt, id):
    filename = secure_filename(file.filename)
    path_base = os.path.join(app.config['UPLOAD_FOLDER'], 'posts')
    path_id = os.path.join(path_base, id)
    path_photo = os.path.join(path_id, opt)

    try:
        os.mkdir(path_id)
    except OSError:
        pass
    try:
        os.mkdir(path_photo)
    except OSError:
        pass

    file.save(os.path.join(path_photo, filename))

    create_thumb(path_photo, filename)

    return 'ok'


@app.route('/upload_user', methods=['GET', 'POST'])
def upload_user():
    if request.method == 'POST':
        file = request.files['file']
        return upload_for_users(file)
    return html_form

def upload_for_users(file):
    filename = secure_filename(file.filename)

    name = filename[0:filename.index('.')]

    for _file in os.listdir(os.path.join(app.config['UPLOAD_FOLDER'], 'users')):
        if fnmatch.fnmatch(_file, str(name)+'.*'):
            os.remove(os.path.join(app.config['UPLOAD_FOLDER'], 'users', _file))

    for _file in os.listdir(os.path.join(app.config['UPLOAD_FOLDER'], 'users')):
        if fnmatch.fnmatch(_file, 'thumb_'+str(name)+'.*'):
            os.remove(os.path.join(app.config['UPLOAD_FOLDER'], 'users', _file))


    path_base = os.path.join(app.config['UPLOAD_FOLDER'], 'users')

    file.save(os.path.join(path_base, filename))
    create_thumb(path_base, filename)



    return 'ok'

@app.route('/upload_tour', methods=['GET', 'POST'])
def upload_tour():
    if request.method == 'POST':
        text = request.form.get('text')
        id = request.form.get('tour')

        path_base = os.path.join(app.config['UPLOAD_FOLDER'], 'posts')
        path_id = os.path.join(path_base, id)
        path_tour = os.path.join(path_id, 'tour.json')

        try:
            os.mkdir(path_id)
        except OSError:
            pass

        with open(path_tour, 'w') as f:
            f.write(text)

        return 'ok'
    return '''
            <!doctype html>
            <title>Upload new File</title>
            <h1>Upload new File</h1>
            <form method=post enctype=multipart/form-data>
              <input type=text name=text>
              <input type=text name=tour>
              <input type=submit value=Upload>
            </form>
            '''



def login_sys(_login, _password):
    #dec_pass = decrypt(password)
    _login = str(_login)
    _password = str(_password)
    user = models.User.query.filter_by(login=_login).first()
    if user != None:
        correct = check_password_hash(user.password, _password)
        return user.id if correct else -1
    return -2


@app.route('/login', methods=['POST', 'GET'])
def login():
    reqData = request.form
    return json.dumps(login_sys(reqData.get('login'), reqData.get('password')))


def json_serial(obj):
    if isinstance(obj, (datetime, date)):
        return obj.isoformat()
    raise TypeError("Type %s is not JSON serializable" % type(obj))


@app.route('/get_post', methods=['POST', 'GET'])
def get_post():
    reqData = request.form
    tour_id = int(reqData.get('tour_id'))
    user_id = int(reqData.get('user_id'))

    tour = models.Tour.query.get(tour_id)
    user = models.User.query.get(tour.user_id)

    userData = user.get()
    tourData = tour.get()
    data = {'id' : tour.user_id,
            'name': userData['name'],
            'description': tourData['desc'],
            'tags': tourData['tags'],
            'geotag': tourData['geotag'],
            'time': tourData['time'],
            'comments': tourData['comments'],
            'likes': len(tourData['likes']),
            'liked': user_id in tourData['likes']}
    return json.dumps(data, separators=(',', ':'), default=json_serial)


@app.route('/get_comments/<int:tour_id>')
def get_comments(tour_id):
    tour = models.Tour.query.get(tour_id)
    res = {'comments':tour.comments()}
    return json.dumps(res)


@app.route('/get_likes/<int:tour_id>')
def get_likes(tour_id):
    tour = models.Tour.query.get(tour_id)
    return json.dumps(tour.likes())


@app.route('/get_profile', methods=['POST', 'GET'])
def get_profile():
    reqData = request.form
    viewer = int(reqData.get('viewer_id'))
    user_id = int(reqData.get('user_id'))

    user = models.User.query.get(user_id)
    userData = user.get()
    userProfile = user.profile()
    subscriptions = models.Subscription.query.filter_by(subscriber_id=user_id)
    subscribers = models.Subscription.query.filter_by(user_id=user_id)
    tours = models.Tour.query.filter_by(user_id=user_id)
    data = {'id': userData["id"],
            'name': userData['name'],
            'login': userData['login'],
            'bio': userProfile['bio'],
            'url': userProfile['url'],
            'subscriptions': subscriptions.count(),
            'subscribers': subscribers.count(),
            'tours': tours.count(),
            'subbed': viewer in [x.subscriber_id for x in subscribers]}
    return json.dumps(data)


@app.route('/get_posts/<int:user_id>')
def get_posts(user_id):
    user = models.User.query.get(user_id)
    res = {'posts':user.posts()}
    return json.dumps(res)


@app.route('/get_feed/<int:user_id>')
def get_feed(user_id):
    feed = models.generate_feed(user_id)
    res = {'posts':feed}
    return json.dumps(res)

@app.route('/search_user/<string:key>')
def search_user(key):
    users = models.search_user(key)
    res = {'users':users}
    return json.dumps(res)

@app.route('/search_post/<string:key>')
def search_post(key):
    posts = models.search_post(key)
    res = {'posts':posts}
    return json.dumps(res)


@app.route('/create_profile', methods=['POST', 'GET'])
def create_profile():
    reqData = request.args
    return json.dumps(models.create_user(
        reqData.get('login'),
        reqData.get('password'),
        reqData.get('name'),
        reqData.get('bio'),
        reqData.get('url')))


@app.route('/create_tour', methods=['POST', 'GET'])
def create_tour():
    reqData = request.form
    if not check_password(reqData.get('user_id', ''), reqData.get('password', '')) == -1:
        return json.dumps(models.create_tour(
            reqData.get('user_id'),
            reqData.get('geotag'),
            reqData.get('desc'),
            reqData.get('tags'),
            reqData.get('time')))
    return '-1'

@app.route('/delete_tour', methods=['POST', 'GET'])
def delete_tour():
    reqData = request.form
    if not check_password(reqData.get('user_id', '0'), reqData.get('password', '')) == -1:

        try:
            dir_name = os.path.join(app.config['UPLOAD_FOLDER'], 'posts', reqData.get('id'))
            for file in os.listdir(dir_name):
                file_path = os.path.join(dir_name, file)
                if os.path.isfile(file_path):
                    os.remove(file_path)

            panos_dir = os.path.join(dir_name, "panoramas")
            try:
                for file in os.listdir(panos_dir):
                    file_path = os.path.join(panos_dir, file)
                    if os.path.isfile(file_path):
                        os.remove(file_path)
                os.rmdir(panos_dir)
            except:
                pass

            photos_dir = os.path.join(dir_name, "photos")
            try:
                for file in os.listdir(photos_dir):
                    file_path = os.path.join(photos_dir, file)
                    if os.path.isfile(file_path):
                        os.remove(file_path)
                os.rmdir(photos_dir)
            except:
                pass

            os.rmdir(dir_name)
        except:
            pass

        return json.dumps(models.delete_tour(
            reqData.get('id')))
    return '-1'

@app.route('/create_comment', methods=['POST', 'GET'])
def create_comment():
    reqData = request.form
    if not check_password(reqData.get('user_id', ''), reqData.get('password', '')) == -1:
        return json.dumps(models.create_comment(
            reqData.get('user_id'),
            reqData.get('tour_id'),
            reqData.get('text')))
    return '-1'


@app.route('/create_like', methods=['POST', 'GET'])
def create_like():
    reqData = request.form
    if not check_password(reqData.get('user_id', ''), reqData.get('password', '')) == -1:
        user_id = int(reqData.get('user_id'))
        tour_id = int(reqData.get('tour_id'))
        likes = models.Like.query.filter_by(user_id=user_id)
        for like in likes:
            if like.tour_id == tour_id:
                return '-1'
        return json.dumps(models.create_like(
            user_id,
            tour_id))
    return '-1'


@app.route('/create_subscription', methods=['POST', 'GET'])
def create_subscription():
    reqData = request.form
    if not check_password(reqData.get('subscriber_id', ''), reqData.get('password', '')) == -1:
        user_id = int(reqData.get('user_id'))
        subscriber_id = int(reqData.get('subscriber_id'))
        return json.dumps(models.create_subscription(
        user_id,
        subscriber_id))
    return '-1'

@app.route('/delete_subscription', methods=['POST', 'GET'])
def delete_subscription():
    reqData = request.form
    if not check_password(reqData.get('subscriber_id', ''), reqData.get('password', '')) == -1:
        user_id = int(reqData.get('user_id'))
        subscriber_id = int(reqData.get('subscriber_id'))
        return json.dumps(models.delete_subscription(
        user_id,
        subscriber_id))
    return '-1'

@app.route('/validate_email', methods=['POST', 'GET'])
def validate_email():
    reqData = request.args
    return str(models.validate_email(reqData.get('id')))


@app.route('/update_user', methods=['POST', 'GET'])
def update_user():
    reqData = request.form
    if not check_password(reqData.get('id', ''), reqData.get('password', '')) == -1:
        return json.dumps(models.edit_user(
            reqData.get('id', ''),
            reqData.get('login', ''),
            '',
            reqData.get('name', ''),
            reqData.get('bio', ''),
            reqData.get('url', '')))
    return '-1'