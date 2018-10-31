from run import db, mail, app
from flask_mail import Message
from werkzeug.security import generate_password_hash, check_password_hash

class User(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    login = db.Column(db.String(64), unique=True)
    password = db.Column(db.String(128))
    name = db.Column(db.String(64))
    bio = db.Column(db.String(256))
    url = db.Column(db.String(128)) #aka email
    validated = db.Column(db.Boolean) #aka email

    def get(self):
        return {'id': self.id,
                'name': self.name,
                'login': self.login}

    def profile(self):
        return {'bio': self.bio,
                'url': self.url}

    def posts(self):
        posts = Tour.query.filter_by(user_id=self.id)
        return [x.id for x in posts[::-1]]

def search_user(key):
    users = User.query.filter(User.name.like('%'+key+'%')).all()
    return [{'id':x.id, 'login':x.login, 'name':x.name} for x in users]

def send_mail(to, name, id):
    try:
        msg = Message(subject="Successful Circum registration",
            sender=app.config["MAIL_USERNAME"],
            recipients=[to], # replace with your email for testing
            body="Dear {0},\nThanks for signing up for Circum!\nClick on this link to validate your email:\ngurtle.pythonanywhere.com/validate_email?id={1}".format(name, id))
        mail.send(msg)
        return 0
    except:
        return -1

def create_user(login, password, name, bio, url):
    newUser = User(login=login, password=generate_password_hash(password),
                   name=name, bio=bio, url=url, validated=False)
    db.session.add(newUser)
    db.session.commit()
    db.session.refresh(newUser)
    send_mail(url, login, newUser.id)
    return newUser.id

def validate_email(id):
    _id = int(id)
    user = User.query.get(_id)
    if user == None:
        return -1
    user.validated = True;
    db.session.commit()
    return 'success'

def edit_user(id, login, password, name, bio, url):
    user = User.query.get(id)
    if user == None:
        return -1

    if login!= '':
        user.login = login
    if password!= '':
        user.password = password
    if name!= '':
        user.name = name
    if bio!= '':
        user.bio = bio
    if url!= '':
        user.url = url

    db.session.commit()

    return 0


class Tour(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    geotag = db.Column(db.String(64))
    desc = db.Column(db.Text)
    tags = db.Column(db.String(64))
    time = db.Column(db.DateTime)

    def __repr__(self):
        return self.id

    def get(self):
        return {'desc': self.desc,
                'tags': self.tags,
                'time': self.time,
                'geotag': self.geotag,
                'comments':len(self.comments()),
                'likes':self.likes()}

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

def delete_tour(_id):
    Comment.query.filter_by(tour_id=_id).delete()
    Like.query.filter_by(tour_id=_id).delete()
    Tour.query.filter_by(id=_id).delete()
    db.session.commit()
    return '0'

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

    def __str__(self):
        return str(self.id)


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
    subscription = Subscription.query.filter_by(user_id=user_id).filter_by(subscriber_id=subscriber_id).first()
    if not subscription == None:
        return '-1'

    newSub = Subscription(user_id=user_id, subscriber_id=subscriber_id)
    db.session.add(newSub)
    db.session.commit()
    db.session.refresh(newSub)
    return newSub.id

def delete_subscription(user_id, subscriber_id):
    sub = Subscription.query.filter_by(user_id=user_id).filter_by(subscriber_id=subscriber_id).first()
    if not sub == None:
        db.session.delete(sub)
        db.session.commit()
        return '0'
    return '-1'


def generate_feed(user_id):
    postsList = []

    posts = Tour.query.filter_by(user_id=user_id)
    posts = [x.id for x in posts if x.id not in postsList]
    map(lambda x: postsList.append(x), posts)

    posts = db.session.query(Tour).order_by(Tour.id.desc()).limit(20)
    posts = [x.id for x in posts if x.id not in postsList]
    map(lambda x: postsList.append(x), posts)

    subscriptions = Subscription.query.filter_by(subscriber_id=user_id)
    for subscription in subscriptions:
        posts = Tour.query.filter_by(user_id=subscription.user_id)
        posts = [x.id for x in posts if x.id not in postsList]
        map(lambda x: postsList.append(x), posts)
    return sorted(postsList)[::-1]
