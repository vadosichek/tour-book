from app import db


class User(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    login = db.Column(db.String(64), unique=True)
    password = db.Column(db.String(128))
    name = db.Column(db.String(64))
    bio = db.Column(db.String(256))
    url = db.Column(db.String(128))
    pic = db.Column(db.String(32))

    def get(self):
        return {'id': self.id,
                'name': self.name,
                'pic': self.pic}

    def profile(self):
        return {'bio': self.bio,
                'url': self.url}

    def posts(self):
        posts = Tour.query.filter_by(user_id=self.id)
        return list(map(lambda x: {'id': x.id, 'pic': x.pic}, posts))


def create_user(login, password, name, bio, url, pic):
    newUser = User(login=login, password=password,
                   name=name, bio=bio, url=url, pic=pic)
    db.session.add(newUser)
    db.session.commit()


def search_user(key):
    foundByLogin = User.query.filter(User.login.ilike(key))
    foundByName = User.query.filter(User.name.ilike(key))
    foundByBio = User.query.filter(User.bio.ilike(key))
    foundByUrl = User.query.filter(User.url.ilike(key))
    result = []
    map(lambda x: result.append(
        {'id': x.id, 'login': x.login, 'name': x.name, 'pic': x.pic}), foundByLogin)
    map(lambda x: result.append(
        {'id': x.id, 'login': x.login, 'name': x.name, 'pic': x.pic}), foundByName)
    map(lambda x: result.append(
        {'id': x.id, 'login': x.login, 'name': x.name, 'pic': x.pic}), foundByBio)
    map(lambda x: result.append(
        {'id': x.id, 'login': x.login, 'name': x.name, 'pic': x.pic}), foundByUrl)
    return result


class Tour(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    path = db.Column(db.String(32), unique=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    geotag = db.Column(db.String(64))
    desc = db.Column(db.Text)
    tags = db.Column(db.String(64))
    size = db.Column(db.Integer)
    date = db.Column(db.Date)
    pic = db.Column(db.String(32))

    def __repr__(self):
        return self.pic

    def get(self):
        return {'pic': self.pic,
                'desc': self.desc,
                'tags': self.tags,
                'date': self.date,
                'geotag': self.geotag}

    def comments(self):
        comments = Comment.query.filter_by(tour_id=self.id)
        return list(map(lambda x: {'user_name': User.query.get(x.user_id).name, 'text': x.text}, comments))


def create_tour(path, user_id, geotag, desc, tags, size, date, pic):
    newTour = Tour(path=path, user_id=user_id, geotag=geotag,
                   desc=desc, tags=tags, size=size, date=date, pic=pic)
    db.session.add(newTour)
    db.session.commit()


class Comment(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    tour_id = db.Column(db.Integer, db.ForeignKey('tour.id'))
    text = db.Column(db.String(128))


def create_comment(user_id, tour_id, text):
    newComment = Comment(user_id=user_id, tour_id=tour_id, text=text)
    db.session.add(newComment)
    db.session.commit()


class Like(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    tour_id = db.Column(db.Integer, db.ForeignKey('tour.id'))


def create_like(user_id, tour_id):
    newLike = Like(user_id=user_id, tour_id=tour_id)
    db.session.add(newLike)
    db.session.commit()


class Subscription(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    subscriber_id = db.Column(db.Integer, db.ForeignKey('user.id'))


def create_subscription(user_id, subscriber_id):
    newSub = Subscription(user_id=user_id, subscriber_id=subscriber_id)
    db.session.add(newSub)
    db.session.commit()


def generate_feed(user_id):
    postsList = []
    subscriptions = Subscription.query.filter_by(subscriber_id=user_id)
    for subscription in subscriptions:
        posts = Tour.query.filter_by(user_id=subscription.user_id)
        map(lambda x: postsList.append(x.id), posts)
    return postsList
