import Cookie from 'js-cookie';

const GetCookie = cookieName => {
	return Cookie.get(cookieName);
};

export default GetCookie;
