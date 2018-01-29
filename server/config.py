import os
basedir = os.path.abspath(os.path.dirname(__file__))

SQLALCHEMY_DATABASE_URI = 'mysql://root:7710024vV@localhost:3306/tour_book'
SQLALCHEMY_MIGRATE_REPO = os.path.join(basedir, 'dp_repository')
SQLALCHEMY_TRACK_MODIFICATIONS = True