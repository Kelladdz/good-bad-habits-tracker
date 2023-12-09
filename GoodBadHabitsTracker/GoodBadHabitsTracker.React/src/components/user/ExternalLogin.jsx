import {GoogleLogin} from 'react-google-login';

const clientId = "238617088969-cugrh0v8a7b0vfti8c42rjio7spms255.apps.googleusercontent.com";

export default function ExternalLogin() {
    const onSuccess = (res) => console.log("Success!", res.profileObj);
    const onFailure = (res) => console.log("Fail!", res)

    return (
        <div id="signInButton">
            <GoogleLogin 
                clientId={clientId}
                buttonText='Login'
                onSuccess={onSuccess}
                onFailure={onFailure}
                cookiePolicy={'single_host_origin'}
                isSignedIn={true}
                redirectUri=''
            />
        </div>
    )
}