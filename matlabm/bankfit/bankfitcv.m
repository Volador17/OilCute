function [result,result2,minSQ,TQ]=bankfitcv(x,pr,wind,wave1,wave2)
% �������Ϸ�������֤
% ���룺x������ף�pr����������ʣ�wind���ж�����Ƿ��ֽ����ƶ����ϵ����ʶ���������Ĵ��ڿ�ȣ�%
%wave1,�ж�����Ƿ��ֽ����ƶ����ϵ����ʶ���������Ĺ��������ʼλ�ã�wave2���ж�����Ƿ��ֽ����ƶ����ϵ����ʶ���������Ĺ�����������λ�ã�
% �����result��������ʾ��minSQ����С�ƶ����ϵ��,TQ��ʶ�������
% result����300+1��*���ʣ���1��Ϊ��ʶ����Ʒ�����ʣ���2�������������У�Ϊʶ������Ŀ���Ʒ�����ʣ�������������
% result2��300*����Ʒ��Ŀ��ÿһ�дӵ�һ�е����������У�Ϊ������ϵĿ���Ʒ�ı���
% minSQ��1*У������Ʒ��Ŀ
% TQ��1*У������Ʒ��Ŀ
[m,n]=size(x);
for ii=1:n
    y=x(:,ii);
    xx=x;    
	xx(:,ii)=[];
    opr=pr;   
	opr(:,ii)=[];
	[ratio,ad]=bankfit(xx,y);
	% �ж��Ƿ���ϳ��
	nx=xx(:,ad);
	yfit=nx*ratio';
	[T,SQ]=mwcorr(yfit(wave1:wave2,:),y(wave1:wave2,:),wind);
	TQ(ii)=T;
	minSQ(ii)=min(SQ);
	result((ii-1)*300+1,:)=pr(:,ii)';
	result2(1,ii)=ratio(1);
	[m1,n2]=size(ad);
	for j=1:n2
		result((ii-1)*300+j+1,:)=opr(:,ad(j))';
		result2(j,ii)=ratio(j);
	end
end