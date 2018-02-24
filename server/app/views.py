from app import app, models, db
from flask import request
import json
from datetime import date, datetime


def json_serial(obj):
    if isinstance(obj, (datetime, date)):
        return obj.isoformat()
    raise TypeError("Type %s is not JSON serializable" % type(obj))


@app.route('/')
@app.route('/index')
def index():
    return "Hello world!"


@app.route('/get_tour/<int:tour_id>')
def get_tour(tour_id):
    tour = models.Tour.query.get(tour_id)
    user = models.User.query.get(tour.user_id)
    return json.dumps({'user': user.get(), 'tour': tour.get()}, separators=(',', ':'), default=json_serial)


@app.route('/get_comments/<int:tour_id>')
def get_comments(tour_id):
    tour = models.Tour.query.get(tour_id)
    return json.dumps(tour.comments())


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


@app.route('/create_profile', methods=['POST', 'GET'])
def create_profile():
    reqData = request.args
    models.create_user(
        reqData.get('login'), 
        reqData.get('password'), 
        reqData.get('name'), 
        reqData.get('bio'), 
        reqData.get('url'), 
        reqData.get('pic'))
    return "OK"