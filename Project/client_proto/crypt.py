from Crypto.PublicKey import RSA


key = None
with open('key.pem','r') as f:
    key = RSA.importKey(f.read())


def encrypt(data):
    return key.encrypt(str(data), 32)

def decrypt(data):
    return key.decrypt(str(data))