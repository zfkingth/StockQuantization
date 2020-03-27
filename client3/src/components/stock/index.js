


import React from "react";

import styled from 'styled-components'

import StockTable from './StockTable'

import LinearDeterminate from './LinearDeterminate'
import TaskStatusLabel from './TaskStatusLabel'
import ConnectionStatusLabel from './ConnectionStatusLabel'

const TwoColumnsWrap = styled.div`
 
  flex-direction:row;
  display: flex;
  justify-content: flex-start;
  align-items: flex-start;
`
const MulRowsWrap = styled.div`
 
  flex-direction:column;
  display: flex;
  justify-content: flex-start;
  align-items: center;
`


const HelloWorld = (FilterForm) => {

  return (
    <TwoColumnsWrap>
      <MulRowsWrap>
        <FilterForm />
        <ConnectionStatusLabel/>
        <LinearDeterminate />
        <TaskStatusLabel />
      </MulRowsWrap>
      <StockTable />
    </TwoColumnsWrap>
  )
}

export default HelloWorld

