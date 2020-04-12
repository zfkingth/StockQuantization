
import React from "react";

import styled from 'styled-components'



import UserTable from './ManageUsersTable'


const MulRowsWrap = styled.div`
 
  flex-direction:column;
  display: flex;
  justify-content: flex-start;
  align-items: center;
`


const ManageControl = () => {

  return (
      <MulRowsWrap>
          <UserTable/>
      </MulRowsWrap>
  )
}

export default ManageControl

