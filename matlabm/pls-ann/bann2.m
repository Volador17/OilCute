function net=bann2(x,y,xx,yy,trainf,hm,f1,f2,tn,traino)

%�м�ؼ���������ѵ������
%net:���ѵ��50�����ڵõ�������������
%x:У�������׾���
%y:У��������������
%xx:��ؼ����׾���
%yy����ؼ����ʾ���
%trainf:ѵ��������ȡֵΪtraingd��traingdm����trainbfg��trainlm�е�һ��
%hm�������ڵ�����
%f1:��һ�㴫�ݺ�����ȡֵΪtansig,logsig,purelin�е�һ����
%f2:�ڶ��㴫�ݺ�����ȡֵΪtansig,logsig,purelin�е�һ����
%tn:ѵ��������
%traino:ѵ��Ŀ��.

aa=inf;
nn=[];
v.P=xx;
v.T=yy;
for i=1:50
    net=newff(minmax(x),[hm,1],{f1,f2},trainf);
    net.trainParam.lr=0.0002;
    net.trainParam.epochs = tn;
    net.trainParam.goal = traino;
    net=train(net,x,y,[],[],v);
    a1=sim(net,x);
    seca=(sumsqr(y-a1)/(length(a1)-1)).^0.5;
    a2=sim(net,xx);
    sepa=(sumsqr(yy-a2)/(length(a2)-1)).^0.5;
    summ=seca+sepa;
    if summ<aa
        aa=summ;
        nn=net;
    end
end 
 
net=nn;