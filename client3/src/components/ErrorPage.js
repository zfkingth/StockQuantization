import './nav1.css';
import React from 'react';
import { connectTo } from '../utils/generic'

class errorControl extends React.PureComponent {
    render() {
        return (
            <h1>{this.props.errorInfo}</h1>

        )

    }


};

export default connectTo(
    state => ({ 
        errorInfo: 
        state.navigation.errorInfo.toString() }),
    {},
    errorControl
)