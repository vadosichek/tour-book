from app import app, models
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
    userData = user.get()
    tourData = tour.get()
    data = {'name': userData['name'],
            'pic': userData['pic'],
            'preview': tourData['pic'],
            'description': tourData['desc'],
            'tags': tourData['tags'],
            'geotag': tourData['geotag'],
            'date': tourData['date']}
    return json.dumps(data, separators=(',', ':'), default=json_serial)


@app.route('/get_comments/<int:tour_id>')
def get_comments(tour_id):
    tour = models.Tour.query.get(tour_id)
    return json.dumps(tour.comments())


@app.route('/get_profile/<int:user_id>')
def get_profile(user_id):
    user = models.User.quety.get(user_id)
    userData = user.get().append(user.profile)
    subscriptions = models.Subscription.query.filter_by(subcriber_id=user_id)
    subscribers = models.Subscription.query.filter_by(user_id=user_id)
    tours = models.Tour.query.filter_by(user_id=user_id)
    data = {'user': userData,
            'subscriptions': len(subscriptions),
            'subscribers': len(subscribers),
            'tours': len(tours)}
    return json.dump(data)
