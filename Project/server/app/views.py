from app import app, models, db
from flask import request
import json
from datetime import date, datetime


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
            'pic': userData['pic'],
            'preview': tourData['pic'],
            'description': tourData['desc'],
            'tags': tourData['tags'],
            'geotag': tourData['geotag'],
            'time': tourData['time']}
    return json.dumps(data, separators=(',', ':'), default=json_serial)


@app.route('/get_comments/<int:tour_id>')
def get_comments(tour_id):
    tour = models.Tour.query.get(tour_id)
    return json.dumps(tour.comments())


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
            'pic': userData['pic'],
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
    return json.dumps(feed)


@app.route('/create_profile', methods=['POST', 'GET'])
def create_profile():
    reqData = request.args
    return json.dumps(models.create_user(
        reqData.get('login'),
        reqData.get('password'),
        reqData.get('name'),
        reqData.get('bio'),
        reqData.get('url'),
        reqData.get('pic')))


@app.route('/create_tour', methods=['POST', 'GET'])
def create_tour():
    reqData = request.args
    return json.dumps(models.create_tour(
        reqData.get('path'),
        reqData.get('user_id'),
        reqData.get('geotag'),
        reqData.get('desc'),
        reqData.get('tags'),
        reqData.get('size'),
        reqData.get('time'),
        reqData.get('pic')))


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
    return json.dumps(models.create_like(
        reqData.get('user_id'),
        reqData.get('tour_id')))


@app.route('/create_subscription', methods=['POST', 'GET'])
def create_subscription():
    reqData = request.args
    return json.dumps(models.create_subscription(
        reqData.get('user_id'),
        reqData.get('subscriber_id')))