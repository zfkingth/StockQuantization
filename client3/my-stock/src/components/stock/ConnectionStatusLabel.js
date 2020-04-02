import React from 'react';

import { isConnected } from '../../utils/forms'
import { connectTo } from '../../utils/generic';

import styled from "styled-components";

const StyledLabel = styled.label`
 
  width: 220;
  color: red;
 
`


class ConnectionStatusLabel extends React.PureComponent {

    render() {
        const { hasError } = this.props;
        return (

            hasError &&
            <StyledLabel  >失去连接，请刷新页面。 </StyledLabel>


        );
    }
}


export default connectTo(
    state => ({
        hasError: !isConnected(state),
    }),
    {},
    ConnectionStatusLabel);
