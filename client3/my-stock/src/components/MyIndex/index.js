
import React from "react";

import styled from 'styled-components'



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
        <p>指标主页面</p>

      </MulRowsWrap >
    )
  }
}
