import useNavigation from '../../hooks/useNavigation';
import { useLocation } from 'react-router-dom';
import axios from 'axios';
import { useEffect, useState } from 'react';
import qs from 'qs';
import { jwtDecode } from 'jwt-decode';
import { auto } from '@popperjs/core';

export default function GoogleLogoutCallback() {
	const [userName, setUserName] = useState('');
	const [userEmail, setUserEmail] = useState('');
	const [userPicture, setUserPicture] = useState('');
	const [idToken, setIdToken] = useState('');
	const [accessToken, setAccessToken] = useState('');
	const [providerKey, setProviderKey] = useState('');
	const { navigate } = useNavigation();
	let location = useLocation();

	const externalLogout = async () => {
		const response = await axios
			.post(
				'https://localhost:7154/api/auth/external-logout',
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
