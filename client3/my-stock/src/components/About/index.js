
import React from "react";

import styled from 'styled-components'

import DateInfo from './DateInfo';
import ServerVersionInfo from './ServerVersionInfo';



const MulRowsWrap = styled.div`
 
  flex-direction:column;
  display: flex;
  justify-content: flex-start;
  align-items: center;
`



export default class tempcontrol extends React.PureComponent {
  render() {
    return (
      <MulRowsWrap>
        <p>页客户端程序版本:3.0.0</p>
          <ServerVersionInfo/>
          <DateInfo/>
      </MulRowsWrap >
    )
  }
}
