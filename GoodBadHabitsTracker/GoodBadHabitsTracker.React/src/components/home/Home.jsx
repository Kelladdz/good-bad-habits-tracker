import { useEffect, useState } from 'react';
import useNavigation from '../../hooks/useNavigation';
import Cookies from 'js-cookie';

export default function Home() {
	const { navigate } = useNavigation();
	const [token, setToken] = useState({});
	useEffect(() => {
		const userCookie = () => {
			return Cookies.get();
		};
		console.log(userCookie());
		setToken(userCookie);
		console.log(token);
		if (Object.keys(userCookie()).length === 0) {
			navigate('/signin');
		} else navigate('/all-habits');
	});
}
