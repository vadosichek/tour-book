from app import db
from werkzeug.security import generate_password_hash, check_password_hash

class User(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    login = db.Column(db.String(64), unique=True)
    password = db.Column(db.String(128))
    name = db.Column(db.String(64))
    bio = db.Column(db.String(256))
    url = db.Column(db.String(128))

    def get(self):
        return {'id': self.id,
                'name': self.name}

    def profile(self):
        return {'bio': self.bio,
                'url': self.url}

    def posts(self):
        posts = Tour.query.filter_by(user_id=self.id)
        return [x.id for x in posts]

def search_user(key):
    users = User.query.filter(User.name.like('%'+key+'%')).all()
    return [{'id':x.id, 'login':x.login, 'name':x.name} for x in users]

def create_user(login, password, name, bio, url):
    newUser = User(login=login, password=generate_password_hash(password),
                   name=name, bio=bio, url=url)
    db.session.add(newUser)
    db.session.commit()
    db.session.refresh(newUser)
    return newUser.id


class Tour(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    geotag = db.Column(db.String(64))
    desc = db.Column(db.Text)
    tags = db.Column(db.String(64))
    time = db.Column(db.DateTime)

    def __repr__(self):
        return self.pic

    def get(self):
        return {'desc': self.desc,
                'tags': self.tags,
                'time': self.time,
                'geotag': self.geotag,
                'comments':len(self.comments()),
                'likes':len(self.likes())}

    def comments(self):
        comments = Comment.query.filter_by(tour_id=self.id)
        return [{'user_name': User.query.get(comment.user_id).name, 'text': comment.text} for comment in comments]

    def likes(self):
        likes = Like.query.filter_by(tour_id=self.id)
        return [like.user_id for like in likes]

def search_post(key):
    tours = Tour.query.filter(Tour.desc.like('%'+key+'%')).all()
    return [x.id for x in tours]

def create_tour(user_id, geotag, desc, tags, time):
    newTour = Tour(user_id=user_id, geotag=geotag,
                   desc=desc, tags=tags, time=time)
    db.session.add(newTour)
    db.session.commit()
    db.session.refresh(newTour)
    return newTour.id


class Comment(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    tour_id = db.Column(db.Integer, db.ForeignKey('tour.id'))
    text = db.Column(db.String(128))


def create_comment(user_id, tour_id, text):
    newComment = Comment(user_id=user_id, tour_id=tour_id, text=text)
    db.session.add(newComment)
    db.session.commit()
    db.session.refresh(newComment)
    return newComment.id


class Like(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    tour_id = db.Column(db.Integer, db.ForeignKey('tour.id'))


def create_like(user_id, tour_id):
    newLike = Like(user_id=user_id, tour_id=tour_id)
    db.session.add(newLike)
    db.session.commit()
    db.session.refresh(newLike)
    return newLike.id


class Subscription(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    subscriber_id = db.Column(db.Integer, db.ForeignKey('user.id'))


def create_subscription(user_id, subscriber_id):
    newSub = Subscription(user_id=user_id, subscriber_id=subscriber_id)
    db.session.add(newSub)
    db.session.commit()
    db.session.refresh(newSub)
    return newSub.id


def generate_feed(user_id):
    postsList = []
    subscriptions = Subscription.query.filter_by(subscriber_id=user_id)
    for subscription in subscriptions:
        posts = Tour.query.filter_by(user_id=subscription.user_id)
        map(lambda x: postsList.append(x.id), posts)
    return postsList
