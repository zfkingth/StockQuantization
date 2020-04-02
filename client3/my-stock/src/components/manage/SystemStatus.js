
import React from "react";

import styled from 'styled-components'



import SystemTable from './SystemTable'

import TaskNumInfo from './TaskNum'
import RealDataTaskControl from './sendPullRealDataTaskControl';
import BasicDataTaskControl from './sendPullBasicDataTaskControl';
import ClearDateControl from './ClearDateControl';


const MulRowsWrap = styled.div`
 
  flex-direction:column;
  display: flex;
  justify-content: flex-start;
  align-items: center;
`


const ManageControl = () => {

  return (
      <MulRowsWrap>
       <TaskNumInfo/>
       <RealDataTaskControl />
       <br/>
       <BasicDataTaskControl />
       <br/>
       <ClearDateControl/>
       <SystemTable/>
      </MulRowsWrap>
  )
}


export default ManageControl;


