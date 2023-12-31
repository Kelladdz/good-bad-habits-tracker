import Cookies from 'js-cookie';
import { useEffect } from 'react';
import { Button } from 'react-bootstrap';
import useNavigation from '../../hooks/useNavigation';

export default function MainContent() {
	const { navigate } = useNavigation();

	const logout = async () => {
		const response = await axios
			.get('https://localhost:7154/API/Account/Logout', { withCredentials: true })
			.then(res => {
				console.log(res);
				if (res.status === 200) {
					googleLogout();
				}
			});
	};

	const googleLogout = () => {
		window.open('https://localhost:7154/API/Account/GoogleLogout?provider=Google', '_self');
	};

	useEffect(() => {
		const userCookie = () => {
			return Cookies.get('Logged');
		};
		if (userCookie() === undefined) {
			navigate('/signin');
		} else navigate('/all-habits');
	});

	return (
		<>
			<Button onClick={logout}>Logout</Button>
		</>
	);
}
