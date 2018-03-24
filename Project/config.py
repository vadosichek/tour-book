import os
basedir = os.path.abspath(os.path.dirname(__file__))

SQLALCHEMY_DATABASE_URI = 'mysql://b46fb4bee29661:8ebf584a@us-cdbr-iron-east-05.cleardb.net/heroku_1d7008e5f5e06f4'
SQLALCHEMY_MIGRATE_REPO = os.path.join(basedir, 'dp_repository')
SQLALCHEMY_TRACK_MODIFICATIONS = True