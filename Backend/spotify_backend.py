from flask import Flask, jsonify
import requests
import base64
from flask_cors import CORS
import time

app = Flask(__name__)
CORS(app)

CLIENT_ID = "6e8637f6a0234421b80956f9444e0379"
CLIENT_SECRET = "94336921108648fda6f674d5afddcd21"

ACCESS_TOKEN = None
TOKEN_EXPIRES_AT = 0

def get_access_token():
    global ACCESS_TOKEN, TOKEN_EXPIRES_AT
    if ACCESS_TOKEN and time.time() < TOKEN_EXPIRES_AT:
        return ACCESS_TOKEN

    auth_str = f"{CLIENT_ID}:{CLIENT_SECRET}"
    b64_auth = base64.b64encode(auth_str.encode()).decode()

    url = "https://accounts.spotify.com/api/token"
    headers = {
        "Authorization": f"Basic {b64_auth}",
        "Content-Type": "application/x-www-form-urlencoded"
    }
    data = {"grant_type": "client_credentials"}

    resp = requests.post(url, headers=headers, data=data)
    if resp.status_code != 200:
        raise Exception(f"Failed to get Spotify token: {resp.status_code} {resp.text}")

    resp_data = resp.json()
    ACCESS_TOKEN = resp_data["access_token"]
    TOKEN_EXPIRES_AT = time.time() + resp_data["expires_in"] - 10
    return ACCESS_TOKEN

@app.route("/track/<track_id>", methods=["GET"])
def get_track(track_id):
    try:
        token = get_access_token()
    except Exception as e:
        return jsonify({"error": "Failed to get Spotify token", "details": str(e)}), 500

    headers = {"Authorization": f"Bearer {token}"}

    # Track metadata
    track_url = f"https://api.spotify.com/v1/tracks/{track_id}"
    track_resp = requests.get(track_url, headers=headers)
    if track_resp.status_code != 200:
        return jsonify({"error": "Failed to fetch track metadata", "details": track_resp.json()}), track_resp.status_code
    track_data = track_resp.json()

    # Audio features
    features_url = f"https://api.spotify.com/v1/audio-features/{track_id}"
    features_resp = requests.get(features_url, headers=headers)
    if features_resp.status_code == 200:
        features_data = features_resp.json()
        tempo = features_data.get("tempo")
    else:
        print(f"Audio features not available for track {track_id}. Status: {features_resp.status_code}")
        tempo = None

    combined_data = {
        "name": track_data.get("name"),
        "artists": [artist["name"] for artist in track_data.get("artists", [])],
        "tempo": tempo
    }

    return jsonify(combined_data)

if __name__ == "__main__":
    print("Starting Spotify backend on http://127.0.0.1:5000")
    app.run(port=5000)