import os
basedir = os.path.abspath(os.path.dirname(__file__))

SQLALCHEMY_DATABASE_URI = 'mysql://gurtle:7710024vV@gurtle.mysql.pythonanywhere-services.com/gurtle$circum_db'
SQLALCHEMY_MIGRATE_REPO = os.path.join(basedir, 'dp_repository')
SQLALCHEMY_TRACK_MODIFICATIONS = True
UPLOAD_FOLDER = os.path.join(basedir, 'files')

MAIL_SERVER = 'smtp.gmail.com'
MAIL_PORT = 465
MAIL_USE_TLS = False
MAIL_USE_SSL = True
MAIL_USERNAME = 'circum.4pp@gmail.com'
MAIL_PASSWORD = 'LZuL2t$uNZZSQ9*v'