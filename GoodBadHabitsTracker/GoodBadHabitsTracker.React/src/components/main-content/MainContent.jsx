import Cookies from 'js-cookie';
import { useEffect, useState } from 'react';
import { Button } from 'react-bootstrap';
import useNavigation from '../../hooks/useNavigation';

export default function MainContent() {
	const { navigate } = useNavigation();
	const [loginProvider, setLoginProvider] = useState('');

	const logout = async () => {
		const response = await axios.get('https://localhost:7154/api/auth/logout', { withCredentials: true }).then(res => {
			console.log(res);
			if (res.status == 200) {
				navigate('/signin');
			}
			// if (res.status === 200) {
			// 	externalLogout();
			// }
		});
	};

	const externalLogout = async () => {
		// const response = await axios.get('https://localhost:7154/API/Account/ExternalLogout').then(res => console.log(res));
		window.open('https://localhost:7154/api/auth/external-logout', '_self');
	};

	const googleLogout = () => {
		window.open('https://localhost:7154/api/auth/GoogleLogout?provider=Google', '_self');
	};

	const facebookLogout = () => {
		window.open('https://localhost:7154/api/auth/FacebookLogout?provider=Facebook', '_self');
	};

	useEffect(() => {
		const userCookie = () => {
			return Cookies.get('ONSESS');
		};
		if (userCookie() === undefined) {
			navigate('/signin');
		} else navigate('/all-habits');
	});

	useEffect(() => {
		const response = async () =>
			await axios
				.get('https://localhost:7154/api/habits?date=10-01-2024&page=1&limit=10', {
					withCredentials: true,
				})
				.then(res => {
					if (res.status == 401) navigate('/signin');
				});
	});

	return (
		<>
			<Button onClick={logout}>Logout</Button>
		</>
	);
}
