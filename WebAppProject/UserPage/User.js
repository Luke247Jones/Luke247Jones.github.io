import React from 'react';
import { useState, useEffect } from 'react';
import axios from "axios";
import jwtDecode from 'jwt-decode';
import { withRouter } from 'react-router-dom';
import './User.css';
import FormDropdown from '../../Components/FormDropdown';
import useAuth from '../../hooks/useAuth';
import useFindUser from '../../hooks/useFindUser';
import ReactGA from "react-ga";

ReactGA.initialize(process.env.GA_TRACKING_CODE);


function User() {

    useEffect(() => {
        ReactGA.pageview(window.location.pathname + window.location.search);
    });
  // User information state variables
  const [userName, setUserName] = useState("unknown");
  const [userData, setUserData] = useState(null);
  const [orderDataList, setOrderDataList] = useState([]);
  const [boxDataList, setBoxDataList] = useState([]);
  const [apiResponse, setApiResponse] = useState(null);
  const { logoutUser, error } = useAuth();
  const {axiosJWT} = useFindUser();

  // Get User Info when component loads
  React.useEffect(() => {
    // Check if access token is in local storage
    if (sessionStorage.getItem("accessToken") !== null) {
      // Get user id from local storage
      let verification = jwtDecode(sessionStorage.getItem("accessToken"));
      let userId = verification.id;

      // API call for user info
      axiosJWT.post('/users/getUserInfo', {userId})
        .then(response => {
          // Set state of user info to response data
          setUserData(response.data);
          setUserName(response.data.firstname); // Name at top of screen
        }).catch(error => {
          console.log(error.message);
        });
    }
    else {
      // history.push("/signin");
      logoutUser();
    }
  }, []);

  // Get Order Info once when component loads
  React.useEffect(() => {
    // Check if access token is in local storage
    if (sessionStorage.getItem("accessToken") !== null) {
      // Get user id from local storage
      let verification = jwtDecode(sessionStorage.getItem("accessToken"));
      let userId = verification.id;

      // API call for order info
      axiosJWT.post("/users/getOrderInfo", {userId})
        .then(response => {
            if (response.data === "user not found") {
                setApiResponse(response.data);
            }
            else if (response.data.length > 0) {
                // Push data response of order info into array of JSONs
                for (let i = 0; i < response.data.length; i++) {
                    orderDataList.push({
                        order_id: response.data[i].order_id,
                        user_id: response.data[i].user_id,
                        driver_id: response.data[i].driver_id,
                        order_date: convertDate(response.data[i].order_date),
                        order_time: response.data[i].order_time,
                        pick_up_placement_note: response.data[i].pick_up_placement_note,
                        order_comment: response.data[i].order_comment,
                        total: response.data[i].total,
                        order_status: response.data[i].order_status,
                    })
                    // Get box info using order_id
                    getBoxInfo(response.data[i].order_id);
                }
            }
            else {
                setApiResponse("You have 0 orders");
            }      
        }).catch(error => {
          if(error.message === "Cannot read properties of undefined (reading 'accessToken')"){
            logoutUser();
          }
        });
    }
    else {
      logoutUser();
    }
  }, []);

  // Push Box Info using order_id
  function getBoxInfo(orderId) {

    if (sessionStorage.getItem("accessToken") !== null) {
        // API call for box info
        axiosJWT.post("/users/getBoxInfo", {orderId})
        .then(response => {
            boxDataList.push({
                order_id: response.data.order_id,
                box_size: response.data.box_size,
                quantity: response.data.quantity,
            })
            //setBoxDataList(boxDataArray);
        }).catch(error => {
        console.log(error.message);
        });
    }
  }     

  // Convert database date to mm/dd/yy
  function convertDate(isoDate) {
    var date = new Date(isoDate);
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var dt = date.getDate();

    if (dt < 10) {
      dt = '0' + dt;
    }
    if (month < 10) {
      month = '0' + month;
    }

    return ' ' + month + '/' + dt + '/' + year;
  }

  // CSS
  return (
    <div className="body">
      <div className="body">
        <div className="formContainer">
          <div className="formUser">
            <div className="formUserText">
              {userName}
            </div>
          </div>
          <FormDropdown title={"Account Settings"} userInfo={userData}></FormDropdown>
          <FormDropdown title={"Order History"} userInfo={userData} orderInfoList={orderDataList} boxInfoList={boxDataList} apiResponse={apiResponse}></FormDropdown>
          <FormDropdown title={"Upcoming Pickups"} userInfo={userData} orderInfoList={orderDataList} boxInfoList={boxDataList} apiResponse={apiResponse}></FormDropdown>
        </div>
      </div>
    </div>
  );
}

export default withRouter(User);