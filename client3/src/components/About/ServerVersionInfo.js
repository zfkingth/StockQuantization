
import React from "react";

import styled from 'styled-components'


import { get, fetchData } from '../../utils/api'
import * as URL from '../../constants/api'
import CircularProgress from '@material-ui/core/CircularProgress';

import {transformError} from '../../utils/api'


const MulRowsWrap = styled.div`
 
  flex-direction:column;
  display: flex;
  justify-content: flex-start;
  align-items: center;
`

const Loading = styled.div`
 
  display: flex;
  justify-content: center;
  align-items: center;
`
const StyleError = styled.p`
 
color:red;
`


export default class tempcontrol extends React.PureComponent {
  constructor(props) {
    super(props);
    this.state = { receivedData: undefined, error: undefined };
  }

  componentDidMount() {
    this.fetchData();


  }


  fetchData = async function () {
    try {
      const data = await fetchData(get, URL.GETSYSTEMINFO);
      console.log("received data is : " + data);

      this.setState({ receivedData: data });
    } catch (err) {

      console.log("received data error : " + err);
      this.setState({ error: transformError(err) });
    }

  }


  render() {
    return this.state.receivedData ? (
      <MulRowsWrap>
        <p>远程服务器程 序版本：{this.state.receivedData}</p>
      </MulRowsWrap>
    ) : this.state.error ?
        (
          <StyleError>错误：{this.state.error}</StyleError>
        )
        : (
          <Loading>
            <CircularProgress />
          </Loading>
        )

  }
}


