import Cookies from 'js-cookie';
import { useEffect, useState } from 'react';
import { Button } from 'react-bootstrap';
import useNavigation from '../../hooks/useNavigation';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

export default function MainContent() {
	const { navigate } = useNavigation();
	const [loginProvider, setLoginProvider] = useState('');

	const logout = async () => {
		const response = await axios
			.post('https://localhost:7154/api/auth/external-logout', {}, { withCredentials: true })
			.then(res => {
				console.log(res);
				if (res.status === 200) {
				}
			});
	};

	const googleLogout = () => {
		window.open('https://localhost:7154/api/auth/GoogleLogout?provider=Google', '_self');
	};

	const facebookLogout = () => {
		window.open('https://localhost:7154/api/auth/FacebookLogout?provider=Facebook', '_self');
	};

	const getHabits = async () => {
		if (jwtDecode(sessionStorage.getItem('accessToken')).exp > Date.now().valueOf() / 1000) {
			await axios.post(
				'https://localhost:7154/api/auth/refresh-token',
				{
					accessToken: sessionStorage.getItem('accessToken'),
					refreshToken: sessionStorage.getItem('refreshToken'),
				},
				{ withCredentials: true }
			);
		}
		await axios
			.get('https://localhost:7154/api/habits?date=10-01-2024&page=1&limit=10', {
				withCredentials: true,
				headers: { Authorization: `Bearer ${sessionStorage.getItem('accessToken')}` },
			})
			.then(res => {
				console.log(res);
				if (res.status == 401) navigate('/signin');
			});
	};

	useEffect(() => {
		getHabits();
	}, []);

	return (
		<>
			<Button onClick={logout}>Logout</Button>
		</>
	);
}
