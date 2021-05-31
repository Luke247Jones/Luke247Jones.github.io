import React, { Component } from 'react';
import { Ionicons } from '@expo/vector-icons';
import Icon1 from 'react-native-vector-icons/FontAwesome5';
import Icon2 from 'react-native-vector-icons/Entypo';
import Icon3 from 'react-native-vector-icons/Fontisto';
import Icon4 from 'react-native-vector-icons/SimpleLineIcons';
import { Platform, TouchableOpacity, Text, TextInput, StyleSheet, View, Modal, Dimensions } from 'react-native';
import { Container, Header, Content, Form, Item, Input, Label, DatePicker, Picker} from 'native-base';
import AsyncStorage from '@react-native-community/async-storage';
import * as Font from 'expo-font';
import moment from 'moment';
import { usernameCheck, passwordCheck, confirmPasswordCheck, firstNameCheck, lastNameCheck, emailAddressCheck, genderCheck, nationalityCheck, dateOfBirthCheck, inputLineCheck, passwordLineCheck, confirmPasswordLineCheck } from '../storage/ErrorCheck.js';

export default class SignUp extends Component{
  constructor(props) {
    super(props);
    //States for user input and error output alert.
    this.state = {
      screen: Dimensions.get('window'),
      username: '', usernameAlert: '', usernameLine: '#aaaaaa',
      password: '', passwordAlert: '', passwordLine: '#aaaaaa',
      confirmPassword: '', confirmPasswordAlert: '', confirmPasswordLine: '#aaaaaa',
      firstName: '', firstNameAlert: '', firstNameLine: '#aaaaaa',
      lastName: '', lastNameAlert: '', lastNameLine: '#aaaaaa',
      emailAddress: '', emailAddressAlert: '', emailAddressLine: '#aaaaaa',
      gender: '', genderAlert: '', genderLine: '#aaaaaa',
      nationality: '', nationalityAlert: '', nationalityLine: '#aaaaaa',
      dateOfBirth: '', dateOfBirthAlert: '', dateOfBirthLine: '#aaaaaa',
      modal: false, modalIcon: "#22dd22"
    }
    this.handleNameChange = this.handleNameChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.setDate = this.setDate.bind(this);
    this.setGender = this.setGender.bind(this);
  }
  //Set user input for gender and dateOfBirth widgets
  setGender(value: string) {
    this.setState({gender: value});
  }
  setDate(newDate) {
    this.setState({ dateOfBirth: moment(newDate).format('MM/DD/YYYY') });
  }
  //Set rest of user input
  handleNameChange(input) {
    this.setState({ input });
  }
  //Set error alerts and red lines
  handleSubmit() {
    this.setState({usernameAlert: usernameCheck(this.state)}, this.sendSubmit);
    this.setState({usernameLine: inputLineCheck(this.state.username)});
    this.setState({passwordAlert: passwordCheck(this.state)});
    this.setState({passwordLine: passwordLineCheck(this.state.password)});
    this.setState({confirmPasswordAlert: confirmPasswordCheck(this.state)});
    this.setState({confirmPasswordLine: confirmPasswordLineCheck(this.state)});
    this.setState({firstNameAlert: firstNameCheck(this.state)});
    this.setState({firstNameLine: inputLineCheck(this.state.firstName)});
    this.setState({lastNameAlert: lastNameCheck(this.state)});
    this.setState({lastNameLine: inputLineCheck(this.state.lastName)});
    this.setState({emailAddressAlert: emailAddressCheck(this.state)});
    this.setState({emailAddressLine: inputLineCheck(this.state.emailAddress)});
    this.setState({genderAlert: genderCheck(this.state)});
    this.setState({genderLine: inputLineCheck(this.state.gender)});
    this.setState({nationalityAlert: nationalityCheck(this.state)});
    this.setState({nationalityLine: inputLineCheck(this.state.nationality)});
    this.setState({dateOfBirthAlert: dateOfBirthCheck(this.state)});
    this.setState({dateOfBirthLine: inputLineCheck(this.state.dateOfBirth)});
  }
  //If no error alerts, call server function
  sendSubmit = () => {
    const {usernameAlert} = this.state;
    const {passwordAlert} = this.state;
    const {confirmPasswordAlert} = this.state;
    const {firstNameAlert} = this.state;
    const {lastNameAlert} = this.state;
    const {emailAddressAlert} = this.state;
    const {genderAlert} = this.state;
    const {nationalityAlert} = this.state;
    const {dateOfBirthAlert} = this.state;
    if(usernameAlert == '' && passwordAlert == '' && confirmPasswordAlert == '' && firstNameAlert == '' && lastNameAlert == '' && emailAddressAlert == '' && genderAlert == '' && nationalityAlert == '' && dateOfBirthAlert == '')
    {
      this.saveSignUp();
    }
  };
  //Post user info to API. If 500 response, set error message. If 200 response, navigate to HomeScreen.
  saveSignUp = async function() {
    const axios = require('axios').default;
    const data = {
      "username": this.state.username,
      "password": this.state.password,
      "firstName": this.state.firstName,
      "lastName": this.state.lastName,
      "gender": this.state.gender,
      "nationality": this.state.nationality,
      "dateOfBirth": "01/01/1990",
      "emailAddress": this.state.emailAddress
    }
    console.log(data);
    axios({
      method: 'post',
      url: 'https://cognifyxapp.herokuapp.com/user',
      data: data,
      validateStatus: false
      }).then(response => {
        if(JSON.stringify(response.status).includes("500"))
        {
          if(JSON.stringify(response.data).includes("Username already exists!"))
          {
            this.setState({ usernameAlert: "Username already exists" });
          }
          if(JSON.stringify(response.data).includes("Invalid email address given!"))
          {
            this.setState({ emailAddressAlert: 'Invalid Email Address' });
          }
          if(!((JSON.stringify(response.data).includes("Username already exists!")) || (JSON.stringify(response.data).includes("Invalid email address given!"))))
          {
            this.props.navigation.navigate({routeName: 'ServerError'});
          }
        }
        else if(JSON.stringify(response.status).includes("200"))
        {
          this.props.navigation.navigate({routeName: 'HomeScreen'});
          const data = JSON.stringify(response.data);
          const parseData = JSON.parse(data);
          this.saveDataToStorage(parseData.token, parseData.userId, this.state.username);
        }
        else
        {
          this.props.navigation.navigate({routeName: 'ServerError'});
        }
        console.log(response.data);
      }).catch(response => {
        console.log(response.data);
      })
  }
  //Save token, userId, and username to local storage
  saveDataToStorage = (token, userId, username) => {
    AsyncStorage.setItem('userData', JSON.stringify({token: token, userId: userId, username: username}));
  };
  //Screen orientation logic
  getOrientation(){
    if (this.state.screen.width > this.state.screen.height) {
      return 'LANDSCAPE';
    }
    else {
      return 'PORTRAIT';
    }
  }
  getStyle(){
    if (this.getOrientation() === 'LANDSCAPE') {
      return stylesLandscape;
    }
    else {
      return styles;
    }
  }
  onLayout(){
    this.setState({screen: Dimensions.get('window')});
  }
  //UI for SignUp screen
  render() {
    return (
      <View style = {styles.container}>
        {/*Password info modal*/}
        <Modal
          visible= {this.state.modal}
          transparent= {this.state.modal}
          supportedOrientations={['portrait', 'landscape']}
        >
          <View style= {this.getStyle().modalContainerInfo} onLayout = {this.onLayout.bind(this)}>
            <Icon3 name="close" color= 'black' underlayColor= 'blue' size= {35} onPress= {() => this.setState({modal: false, modalIcon: "#22dd22"})} style= {styles.modalIconInfo}/>
            <View style= {this.getStyle().modalInfo} onLayout = {this.onLayout.bind(this)}>
              <View style= {{alignItems: 'center'}}>
                <Text style= {styles.textModal}> Password must contain atleast: </Text>
              </View>
              <View style= {{alignItems: 'flex-start', paddingTop: 20}}>
                <Text style= {styles.textModalList}> 1 uppercase letter </Text>
                <Text style= {styles.textModalList}> 1 lowercase letter </Text>
                <Text style= {styles.textModalList}> 1 number </Text>
                <Text style= {styles.textModalList}> 8 total characters </Text>
              </View>
            </View>
          </View>
        </Modal>
        <Content style= {styles.content}>
        {/*Sign Up Title*/}
        <View style= {styles.title}>
          <Text style= {styles.textTitle}>Let's Get Started</Text>
        </View>
          {/*Sign Up form*/}
          <Form style= {styles.form}>
            {/*Username*/}
            <View style= {[styles.item, {borderColor: this.state.usernameLine}]}>
              <Icon1 name="user-edit" color= '#11dd11' size= {20}/>
              <Input
                style= {styles.inputUsername}
                placeholder= {"Username"}
                placeholderTextColor= {"#888888"}
                onChangeText= {(text) => {this.setState(() => {
                  return {
                    username: text
                  };
                })}}
                maxLength= {32}
              />
            </View>
            <View style= {styles.itemAlert}>
              <Text style= {styles.textError}> {this.state.usernameAlert} </Text>
            </View>
            {/*Password*/}
            <View style= {[styles.item, {borderColor: this.state.passwordLine}]}>
              <Icon1 name="lock" color= 'black' size= {20}/>
              <Input
                style= {styles.input}
                placeholder= {"Password"}
                placeholderTextColor= {"#888888"}
                secureTextEntry= {true}
                onChangeText= {(text) => {this.setState(() => {
                  return {
                    password: text
                  };
                })}}
                maxLength= {32}
              />
              <Icon1 name="exclamation-circle" size = {22} color= {this.state.modalIcon} onPress={() => this.setState({modal: true, modalIcon: '#efefef'})}/>
            </View>
            <View style= {styles.itemAlert}>
              <Text style= {styles.textError}> {this.state.passwordAlert} </Text>
            </View>
            {/*confirmPassword*/}
            <View style= {[styles.item, {borderColor: this.state.confirmPasswordLine}]}>
              <Icon1 name="lock" color= 'black' size= {20}/>
              <Input
                style= {styles.input}
                placeholder= {"Confirm Password"}
                placeholderTextColor= {"#888888"}
                secureTextEntry= {true}
                onChangeText= {(text) => {this.setState(() => {
                  return {
                    confirmPassword: text
                  };
                })}}
              maxLength= {32}
              />
            </View>
            <View style= {styles.itemAlert}>
              <Text style= {styles.textError}> {this.state.confirmPasswordAlert} </Text>
            </View>
            {/*firstName*/}
            <View style= {[styles.item, {borderColor: this.state.firstNameLine}]}>
              <Icon2 name="user" color= '#ee5566' size= {20}/>
              <Input
                style= {styles.input}
                placeholder= {"First Name"}
                placeholderTextColor= {"#888888"}
                onChangeText= {(text) => {this.setState(() => {
                  return {
                    firstName: text
                  };
                })}}
              maxLength= {32}
              />
            </View>
            <View style= {styles.itemAlert}>
              <Text style= {styles.textError}> {this.state.firstNameAlert} </Text>
            </View>
            {/*lastName*/}
            <View style= {[styles.item, {borderColor: this.state.lastNameLine}]}>
              <Icon2 name="user" color= '#ee5566' size= {20}/>
              <Input
                style= {styles.input}
                placeholder= {"Last Name"}
                placeholderTextColor= {"#888888"}
                onChangeText= {(text) => {this.setState(() => {
                  return {
                    lastName: text
                  };
                })}}
                maxLength= {32}
               />
            </View>
            <View style= {styles.itemAlert}>
              <Text style= {styles.textError}> {this.state.lastNameAlert} </Text>
            </View>
            {/*emailAddress*/}
            <View style= {[styles.item, {borderColor: this.state.emailAddressLine}]}>
              <Icon2 name="mail" color= '#aaaaaa' size= {20}/>
              <Input
                style= {styles.input}
                placeholder= {"Email Address"}
                placeholderTextColor= {"#888888"}
                keyboardType= {'email-address'}
                onChangeText= {(text) => {this.setState(() => {
                  return {
                    emailAddress: text
                  };
                })}}
                maxLength= {32}
               />
            </View>
            <View style= {styles.itemAlert}>
              <Text style= {styles.textError}> {this.state.emailAddressAlert} </Text>
            </View>
            {/*gender*/}
            <View style= {[styles.item, {borderColor: this.state.genderLine}]}>
              <Icon1 name="transgender" color= '#ff1111' size= {20}/>
              <View style= {{flexDirection: 'row', paddingLeft: 16}}>
                <Picker
                  mode="dropdown"
                  style={{width: undefined}}
                  placeholder="Gender"
                  placeholderStyle={{ width: 100, color: "#888888", fontSize: 17 }}
                  selectedValue={this.state.gender}
                  onValueChange={this.setGender}
                >
                  <Picker.Item label="Male" value="Male" />
                  <Picker.Item label="Female" value="Female" />
                  <Picker.Item label="Non-binary" value="Non-Binary" />
                </Picker>
                <View style= {{flex: 1, flexDirection: 'row', justifyContent: 'flex-end', alignSelf: 'flex-end', paddingBottom: 10, paddingRight: 36}}>
                  <Icon4 name="arrow-down" size = {22} color= "#888888"/>
                </View>
              </View>
            </View>
            <View style= {styles.itemAlert}>
              <Text style= {styles.textError}> {this.state.genderAlert} </Text>
            </View>
            {/*nationality*/}
            <View style= {[styles.item, {borderColor: this.state.nationalityLine}]}>
              <Icon3 name="earth" color= '#1188bb' size= {20}/>
              <Input
                style= {styles.input}
                placeholder= {"Nationality"}
                placeholderTextColor= {"#888888"}
                onChangeText= {(text) => {this.setState(() => {
                  return {
                    nationality: text
                  };
                })}}
                maxLength= {32}
              />
            </View>
            <View style= {styles.itemAlert}>
              <Text style= {styles.textError}> {this.state.nationalityAlert} </Text>
            </View>
            {/*dateOfBirth*/}
            <View style= {[styles.item, {borderColor: this.state.dateOfBirthLine}]}>
              <Icon1 name="calendar-alt" color= '#ee4411' size= {20}/>
              <View style= {{width: 200, paddingLeft: 20}}>
                <DatePicker
                  formatChosenDate={date => {return moment(date).format('MM/DD/YYYY');}}
                  defaultDate={new Date(2018, 1, 1)}
                  minimumDate={new Date(1900, 1, 1)}
                  maximumDate={new Date(2018, 12, 31)}
                  locale={"en"}
                  timeZoneOffsetInMinutes={undefined}
                  modalTransparent={false}
                  animationType={"fade"}
                  androidMode={"default"}
                  placeHolderText="Date of Birth"
                  textStyle={{ color: "black" }}
                  placeHolderTextStyle={{ color: "#888888", fontSize: 17 }}
                  onDateChange={this.setDate}
                  disabled={false}
                />
              </View>
              <View style= {{flex: 1, flexDirection: 'row', justifyContent: 'flex-end', paddingRight: 20}}>
                <Icon4 name="arrow-down" size = {22} color= "#888888"/>
              </View>
            </View>
            <View style= {styles.itemAlert}>
              <Text style= {styles.textError}> {this.state.dateOfBirthAlert} </Text>
            </View>
          </Form>
          {/*Cancel and Sign Up buttons*/}
          <View style= {styles.buttons}>
              <View style= {this.getStyle().cancel} onLayout = {this.onLayout.bind(this)}>
              <TouchableOpacity
                style= {this.getStyle().cancel} onLayout = {this.onLayout.bind(this)}
                onPress={() => this.props.navigation.navigate({routeName: 'Login'})}
              >
                <Text style= {styles.textCancel}> CANCEL </Text>
              </TouchableOpacity>
              </View>
              <View style= {this.getStyle().signup} onLayout = {this.onLayout.bind(this)}>
              <TouchableOpacity
                style= {this.getStyle().signup} onLayout = {this.onLayout.bind(this)}
                onPress={this.handleSubmit}
              >
                <Text style= {styles.textSignUp}> SIGN UP </Text>
              </TouchableOpacity>
              </View>
          </View>
        </Content>
      </View>
    );
  }
}



const styles = StyleSheet.create({
  signup: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: "center",
    backgroundColor: "#77bb77",
    //padding: 10,
    height: 70,
    width: 180,
    //flex: 1,
    borderRadius: 50,
    borderWidth: 2,
    borderColor: "#77bb77",
    shadowColor: "black",
    shadowRadius: 8,
    shadowOpacity: 0.15
  },
  cancel: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: "center",
    backgroundColor: "white",
    //padding: 10,
    height: 70,
    width: 180,
    //flex: 1,
    borderRadius: 50,
    borderWidth: 2,
    borderColor: "#77bb77",
    shadowColor: "black",
    shadowRadius: 8,
    shadowOpacity: 0.15
  },
  buttons: {
    flex: 1,
    flexDirection: 'row',
    padding: 20,
    paddingTop: 40,
    paddingBottom: 40,
    alignItems: 'stretch',
    alignContent: 'space-between',
    justifyContent: 'space-between'
  },
  input: {
    paddingLeft: 27
  },
  inputUsername: {
    paddingLeft: 20
  },
  container: {
    flex: 1
  },
  item: {
    //backgroundColor: '#DDDDDD',
    flexDirection: 'row',
    justifyContent: 'flex-start',
    alignItems: 'center',
    borderBottomWidth: 1,
    borderColor: '#aaaaaa',
    margin: 15,
    paddingLeft: 10
    //marginBottom: 10
    //padding: 10
  },
  itemGender: {
    //backgroundColor: '#DDDDDD',
    flex: 1,
    flexDirection: 'row',
    justifyContent: 'flex-start',
    alignItems: 'center',
    borderBottomWidth: 1,
    borderColor: '#aaaaaa',
    margin: 15,
    //paddingLeft:1,
    paddingBottom: 5
    //padding: 10
  },
  itemDateOfBirth: {
    //backgroundColor: '#DDDDDD',
    flex: 1,
    flexDirection: 'row',
    justifyContent: 'flex-start',
    alignItems: 'center',
    borderBottomWidth: 1,
    borderColor: '#aaaaaa',
    margin: 15,
    paddingLeft:5,
    paddingBottom: 5
    //padding: 10
  },
  itemAlert: {
    paddingLeft: 10
  },
  modalContainerInfo: {
    backgroundColor: '#ffffff55',
    flex: 1,
    paddingTop: 270,

  },
  modalInfo : {
    backgroundColor: '#ffffff',
    height: 180,
    padding: 30,
    marginTop: 10,
    marginHorizontal: 30,
    borderRadius: 20,
    shadowRadius: 5,
    shadowColor: 'black',
    shadowOpacity: 0.10
  },
  modalIconInfo: {
    alignSelf: 'flex-end',
    paddingRight: 10
  },

  textSignUp: {
    fontSize: 25,
    fontWeight: 'bold',
    color: 'white'
  },
  textCancel: {
    fontSize: 25,
    fontWeight: 'bold',
    color: '#77bb77'
  },
  textError: {
    color: 'red'
  },
  textTitle:{
    fontSize: 25,
    fontWeight: 'bold',
    color: '#555555'
  },
  textModal: {
    fontSize: 16,
    fontWeight: 'bold'
  },
  textModalList: {
    fontSize: 16
  },
  title: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignContent: 'center'
  },
  content: {
    paddingTop: 40
  },
  form: {
    paddingTop: 30
  }
});

const stylesLandscape = StyleSheet.create({
  signup: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: "center",
    backgroundColor: "#77bb77",
    //padding: 10,
    height: 70,
    width: 360,
    //flex: 1,
    borderRadius: 50,
    borderWidth: 2,
    borderColor: "#77bb77",
    shadowColor: "black",
    shadowRadius: 8,
    shadowOpacity: 0.15
  },
  cancel: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: "center",
    backgroundColor: "white",
    //padding: 10,
    height: 70,
    width: 360,
    //flex: 1,
    borderRadius: 50,
    borderWidth: 2,
    borderColor: "#77bb77",
    shadowColor: "black",
    shadowRadius: 8,
    shadowOpacity: 0.15
  },
  buttons: {
    flex: 1,
    flexDirection: 'row',
    padding: 20,
    paddingTop: 40,
    alignItems: 'stretch',
    alignContent: 'space-between',
    justifyContent: 'space-between'
  },
  input: {
    paddingLeft: 27
  },
  inputUsername: {
    paddingLeft: 20
  },
  container: {
    flex: 1
  },
  item: {
    //backgroundColor: '#DDDDDD',
    flexDirection: 'row',
    justifyContent: 'flex-start',
    alignItems: 'center',
    borderBottomWidth: 1,
    borderColor: '#aaaaaa',
    margin: 15,
    paddingLeft: 10
    //marginBottom: 10
    //padding: 10
  },
  itemGender: {
    //backgroundColor: '#DDDDDD',
    flex: 1,
    flexDirection: 'row',
    justifyContent: 'flex-start',
    alignItems: 'center',
    borderBottomWidth: 1,
    borderColor: '#aaaaaa',
    margin: 15,
    //paddingLeft:1,
    paddingBottom: 5
    //padding: 10
  },
  itemDateOfBirth: {
    //backgroundColor: '#DDDDDD',
    flex: 1,
    flexDirection: 'row',
    justifyContent: 'flex-start',
    alignItems: 'center',
    borderBottomWidth: 1,
    borderColor: '#aaaaaa',
    margin: 15,
    paddingLeft:5,
    paddingBottom: 5
    //padding: 10
  },
  itemAlert: {
    paddingLeft: 10
  },
  modalContainerInfo: {
    backgroundColor: '#ffffff55',
    flex: 1,
    paddingTop: 220,
    paddingRight: 40,
    paddingBottom: 10,
    alignItems: 'flex-end',
    justifyContent: 'flex-end'
  },
  modalInfo : {
    backgroundColor: '#ffffff',
    height: 150,
    padding: 20,
    marginTop: 10,
    marginHorizontal: 30,
    borderRadius: 20,
    shadowRadius: 5,
    shadowColor: 'black',
    shadowOpacity: 0.10,
  },
  modalIconInfo: {
    alignSelf: 'flex-end',
    paddingRight: 10
  },

  textSignUp: {
    fontSize: 25,
    fontWeight: 'bold',
    color: 'white'
  },
  textCancel: {
    fontSize: 25,
    fontWeight: 'bold',
    color: '#77bb77'
  },
  textError: {
    color: 'red'
  },
  textTitle:{
    fontSize: 25,
    fontWeight: 'bold',
    color: '#555555'
  },
  textModal: {
    fontSize: 16,
    fontWeight: 'bold'
  },
  textModalList: {
    fontSize: 16
  },
  title: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignContent: 'center'
  },
  content: {
    paddingTop: 40
  },
  form: {
    paddingTop: 30
  }
});
