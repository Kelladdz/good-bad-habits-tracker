import useNavigation from '../../hooks/useNavigation';
import { useLocation } from 'react-router-dom';
import axios from 'axios';
import { useEffect, useState } from 'react';
import qs from 'qs';
import { jwtDecode } from 'jwt-decode';

export default function Callback() {
	const [userName, setUserName] = useState('');
	const [userEmail, setUserEmail] = useState('');
	const [userPicture, setUserPicture] = useState('');
	const [idToken, setIdToken] = useState('');
	const [accessToken, setAccessToken] = useState('');
	const [providerKey, setProviderKey] = useState('');
	const { navigate } = useNavigation();
	let location = useLocation();

	let searchParams = new URLSearchParams(location.search);

	let code = searchParams.get('code');
	let codeVerifier = localStorage.getItem('codeVerifier');
	console.log(code, codeVerifier);

	const tokenRequest = async () => {
		if (accessToken === '') {
			const params = {
				grant_type: 'authorization_code',
				code: code,
				redirect_uri: 'https://localhost:8080/callback-facebook',
				client_id: 'cNRB11SQnB796najkgVTLftkwgkdtNL5',
				code_verifier: codeVerifier,
			};

			const options = {
				method: 'POST',
				headers: { 'content-type': 'application/x-www-form-urlencoded' },
				data: qs.stringify(params),
				url: 'https://dev-d3sgzf7qkeuvnndt.eu.auth0.com/oauth/token',
			};

			const response = await axios(options);
			setIdToken(response.data.id_token);
			console.log(idToken);
			setAccessToken(response.data.access_token);
			console.log(accessToken);

			console.log(response.data);
			console.log(response.status);

			if (response.status === 200) {
				externalLogin();
			}
		} else return;
	};

	const externalLogin = async () => {
		if (accessToken !== '') {
			const response = await axios
				.post(
					'https://localhost:7154/api/auth/external-login?provider=Facebook',
					{},
					{ withCredentials: true, headers: { Authorization: `Bearer ${accessToken}`, Authentication: idToken } }
				)
				.then(res => {
					console.log(res);
					if (res.status === 200) window.close();
				})
				.catch(errs => {
					console.log(errs);
					if (errs.response.status === 401 || errs.response.data.includes('NullReferenceException'))
						errorData = 'Invalid email or password';
				});
		} else return;
	};

	useEffect(() => {
		tokenRequest();
	}, []);

	useEffect(() => {
		if (accessToken !== '') {
			externalLogin(userName, userEmail, userPicture);
		}
	}, [accessToken]);
}
