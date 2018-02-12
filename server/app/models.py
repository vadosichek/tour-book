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
        return list(map(lambda x: {'user': x.user_id, 'text': x.text}, comments))


class Comment(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    tour_id = db.Column(db.Integer, db.ForeignKey('tour.id'))
    text = db.Column(db.String(128))


class Like(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    tour_id = db.Column(db.Integer, db.ForeignKey('tour.id'))


class Subscription(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    user_id = db.Column(db.Integer, db.ForeignKey('user.id'))
    subscriber_id = db.Column(db.Integer, db.ForeignKey('user.id'))
