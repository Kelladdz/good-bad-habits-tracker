import { useContext } from 'react';
import ErrorsContext from '../context/errors';

export default function useErrors() {
	return useContext(ErrorsContext);
}
