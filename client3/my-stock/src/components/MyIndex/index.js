
import React from "react";

import styled from 'styled-components'

import StockIndex from './StockIndex'

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
        <StockIndex />
      </MulRowsWrap >
    )
  }
}
