import Cookies from 'js-cookie';
import { useEffect } from 'react';
import { Button } from 'react-bootstrap';
import useNavigation from '../../hooks/useNavigation';

export default function MainContent({ onLogout }) {
	const { navigate } = useNavigation();

	const logout = async () => {
		onLogout();
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
