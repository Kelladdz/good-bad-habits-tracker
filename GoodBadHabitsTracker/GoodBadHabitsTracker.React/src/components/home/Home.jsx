import { useEffect } from 'react';
import useNavigation from '../../hooks/useNavigation';
import Cookies from 'js-cookie';

export default function Home() {
	const { navigate } = useNavigation();
	
	useEffect(() => {
		const userCookie = () => {
			return Cookies.get('ONSESS');
		};
		console.log(userCookie());
		if (userCookie() === undefined) {
			navigate('/signin');
		} else navigate('/all-habits');
	});
}
