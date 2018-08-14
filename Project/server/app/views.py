from app import app, models, db
from flask import request, send_from_directory, redirect, url_for
import json
from datetime import date, datetime
from werkzeug.security import generate_password_hash, check_password_hash
import os
import fnmatch
#from crypt import decrypt



@app.route('/get_panorama', methods=['GET', 'POST'])
def get_panorama():
    post = request.args.get('id')
    name = request.args.get('name')

    for file in os.listdir(os.path.join(app.config['UPLOAD_FOLDER'], 'posts', str(post), 'panoramas')):
        if fnmatch.fnmatch(file, str(name)+'*'):
            return send_from_directory(os.path.join(app.config['UPLOAD_FOLDER'], 'posts', str(post), 'panoramas'),
                               file)

    return 'err'

@app.route('/get_photo', methods=['GET', 'POST'])
def get_photo():
    post = request.args.get('id')
    name = request.args.get('name')

    for file in os.listdir(os.path.join(app.config['UPLOAD_FOLDER'], 'posts', str(post), 'photos')):
        if fnmatch.fnmatch(file, str(name)+'*'):
            return send_from_directory(os.path.join(app.config['UPLOAD_FOLDER'], 'posts', str(post), 'photos'),
                               file)

    return 'err'

@app.route('/get_user', methods=['GET', 'POST'])
def get_user():
    name = request.args.get('name')

    for file in os.listdir(os.path.join(app.config['UPLOAD_FOLDER'], 'users')):
        if fnmatch.fnmatch(file, str(name)+'*'):
            return send_from_directory(os.path.join(app.config['UPLOAD_FOLDER'], 'users'),
                               file)

    return 'err'


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
    return 'ok'


@app.route('/upload_user', methods=['GET', 'POST'])
def upload_user():
    if request.method == 'POST':
        file = request.files['file']
        return upload_for_users(file)
    return html_form

def upload_for_users(file):
    filename = secure_filename(file.filename)
    path_base = os.path.join(app.config['UPLOAD_FOLDER'], 'users')

    file.save(os.path.join(path_base, filename))
    return redirect(url_for('uploaded_file',
                            filename=filename))





def login_sys(login, password):
    #dec_pass = decrypt(password)
    user = models.User.query.filter_by(login=login).first()
    correct = check_password_hash(user.password, password)
    return user.id if correct else -1


@app.route('/login', methods=['POST', 'GET'])
def login():
    reqData = request.args
    return json.dumps(login_sys(reqData.get('login'), reqData.get('password')))


def json_serial(obj):
    if isinstance(obj, (datetime, date)):
        return obj.isoformat()
    raise TypeError("Type %s is not JSON serializable" % type(obj))


@app.route('/get_post/<int:tour_id>')
def get_post(tour_id):
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
            'likes': tourData['likes']}
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


@app.route('/get_profile/<int:user_id>')
def get_profile(user_id):
    user = models.User.query.get(user_id)
    userData = user.get()
    userProfile = user.profile()
    subscriptions = models.Subscription.query.filter_by(subscriber_id=user_id)
    subscribers = models.Subscription.query.filter_by(user_id=user_id)
    tours = models.Tour.query.filter_by(user_id=user_id)
    data = {'id': userData["id"],
            'name': userData['name'],
            'bio': userProfile['bio'],
            'url': userProfile['url'],
            'subscriptions': subscriptions.count(),
            'subscribers': subscribers.count(),
            'tours': tours.count()}
    return json.dumps(data)


@app.route('/get_posts/<int:user_id>')
def get_posts(user_id):
    user = models.User.query.get(user_id)
    return json.dumps(user.posts())


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
    reqData = request.args
    return json.dumps(models.create_tour(
        reqData.get('user_id'),
        reqData.get('geotag'),
        reqData.get('desc'),
        reqData.get('tags'),
        reqData.get('time')))


@app.route('/create_comment', methods=['POST', 'GET'])
def create_comment():
    reqData = request.args
    return json.dumps(models.create_comment(
        reqData.get('user_id'),
        reqData.get('tour_id'),
        reqData.get('text')))


@app.route('/create_like', methods=['POST', 'GET'])
def create_like():
    reqData = request.args
    user_id = int(reqData.get('user_id'))
    tour_id = int(reqData.get('tour_id'))
    likes = models.Like.query.filter_by(user_id=user_id)
    for like in likes:
        if like.tour_id == tour_id:
            return '-1'
    return json.dumps(models.create_like(
        user_id,
        tour_id))


@app.route('/create_subscription', methods=['POST', 'GET'])
def create_subscription():
    reqData = request.args
    return json.dumps(models.create_subscription(
        reqData.get('user_id'),
        reqData.get('subscriber_id')))